using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rendering3D.Converters;
using Rendering3D.Custom;
using Rendering3D.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace Rendering3D
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Commands
        public RelayCommand ZoomPlusSceneCommand { get; set; }
        public RelayCommand ZoomMinusSceneCommand { get; set; }

        #endregion // Commands

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private BitmapImage scene;
        public BitmapImage Scene
        {
            get { return scene; }
            set { if (scene != value) { scene = value; RaisePropertyChanged("Scene"); } }
        }

        private double zoomProperty;
        public double ZoomProperty
        {
            get { return zoomProperty; }
            set { if (zoomProperty != value) { zoomProperty = value; RaisePropertyChanged("ZoomProperty"); } }
        }
        #endregion // Properties

        #region Fields

        int sceneWidth;
        int sceneHeight;

        double aspect;
        double fov = 1;

        double near = 0.1;
        double far = 1000;

        Matrix<double> M;
        Matrix<double> Scale;
        Matrix<double> T;
        Matrix<double> Zoom;

        Vector<double> C = DenseVector.Build.DenseOfArray(new double[4] { 3.0, 1.5, 0, 0 });

        double rotationChange = Math.PI / 60;

        double xRotation = 0;
        double yRotation = 0;
        double zRotation = Math.PI;

        CustomCube cube1;
        CustomCube cube2;

        Point mouseDownPoint;

        #endregion // Fields

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            ZoomPlusSceneCommand = new RelayCommand(ZoomPlus);
            ZoomMinusSceneCommand = new RelayCommand(ZoomMinus);

            Scale = DenseMatrix.CreateIdentity(4);
            Scale[0, 0] = 450;
            Scale[1, 1] = 450;

            T = DenseMatrix.CreateIdentity(4);

            Zoom = DenseMatrix.CreateIdentity(4);
            Zoom[3, 2] = 30;
            ZoomProperty = 30;
        }
        #endregion // Constructor

        #region Scene
        private void GetSceneSize(object sender, EventArgs e)
        {
            ColumnDefinition sceneColumn = (ColumnDefinition)this.FindName("SceneColumn");
            RowDefinition sceneRow = (RowDefinition)this.FindName("SceneRow");
            sceneWidth = (int)sceneColumn.ActualWidth;
            sceneHeight = (int)sceneRow.ActualHeight;

            PrepareScene();
        }

        private void PrepareScene()
        {
            Image imageControl = (Image)this.FindName("SceneImage");
            imageControl.Width = sceneWidth;
            imageControl.Height = sceneHeight;

            Bitmap bitmap = new Bitmap(sceneWidth, sceneHeight);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, sceneWidth, sceneHeight);
                graph.FillRectangle(Brushes.Black, ImageSize);
            }

            cube1 = new CustomCube(700, 300, 50, 100);
            cube2 = new CustomCube(550, 300, 150, 100);

            bitmap = cube1.Draw(bitmap);
            bitmap = cube2.Draw(bitmap);

            Scene = ImageConverters.Bitmap2BitmapImage(bitmap);

            imageControl.Source = Scene;

            aspect = (double)sceneWidth / (double)sceneHeight;

            M = DenseMatrix.OfArray(new double[,] { { aspect * fov, 0, 0, 0 },
                                                    { 0, fov, 0, 0 },
                                                    { 0, 0, far/(far-near), 1 },
                                                    { 0, 0, (-far*near) / (far-near), 0} });
        }
        #endregion // Scene

        #region Button Methods
        private void ZoomPlus(object o)
        {
            Zoom[3, 2]++;
            // Updating binding
            ZoomProperty++;
            Redraw();
        }
        private void ZoomMinus(object o)
        {
            Zoom[3, 2]--;
            // Updating binding
            ZoomProperty--;
            Redraw();
        }
        #endregion // Button Methods

        #region Mouse Events
        private void SceneMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image sceneImage = (Image)this.FindName("SceneImage");
            mouseDownPoint = e.GetPosition(sceneImage);
        }

        private void SceneMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image sceneImage = (Image)this.FindName("SceneImage");

            Point mouseButtonUpPoint = e.GetPosition(sceneImage);

            int dx = (int)(mouseButtonUpPoint.X - mouseDownPoint.X);
            int dy = (int)(mouseButtonUpPoint.Y - mouseDownPoint.Y);

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                yRotation += rotationChange;
                Redraw();
            }
            else
            {
                xRotation += rotationChange;
                Redraw();
            }
        }

        private void SceneMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom[3, 2]++;
                // Updating binding
                ZoomProperty++;
                Redraw();
            }
            else
            {
                Zoom[3, 2]--;
                // Updating binding
                ZoomProperty--;
                Redraw();
            }
        }
        #endregion // Mouse Events

        #region Key Events

        private void SceneKeyDown(object sender, KeyEventArgs e)
        {
            bool wasControlKeyDown = false;

            if (e.Key == Key.Up || e.Key == Key.W)
            {
                xRotation -= rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                xRotation += rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                yRotation += rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                yRotation -= rotationChange;
                wasControlKeyDown = true;
            }

            if (wasControlKeyDown)
                Redraw();
        }

        #endregion

        private void Redraw()
        {
            Matrix<double> rotationX = DenseMatrix.OfArray(new double[,]
            {
                {1, 0, 0, 0 },
                {0, Math.Cos(xRotation), Math.Sin(xRotation), 0 },
                {0, -Math.Sin(xRotation), Math.Cos(xRotation), 0 },
                {0, 0, 0, 1 }
            });

            Matrix<double> rotationY = DenseMatrix.OfArray(new double[,]
            {
                {Math.Cos(yRotation), 0, -Math.Sin(yRotation), 0 },
                {0, 1, 0, 0, },
                {Math.Sin(yRotation), 0, Math.Cos(yRotation), 0 },
                { 0, 0, 0, 1 }
            });

            Matrix<double> rotationZ = DenseMatrix.OfArray(new double[,]
            {
                {Math.Cos(zRotation), -Math.Sin(zRotation), 0, 0 },
                {Math.Sin(zRotation), Math.Cos(zRotation), 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });

            Matrix<double> newMatrix = DenseMatrix.CreateIdentity(4) * T * rotationY * rotationX * rotationZ * Zoom;

            Bitmap bitmap = ImageConverters.BitmapImage2Bitmap(Scene);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, sceneWidth, sceneHeight);
                graph.FillRectangle(Brushes.Black, ImageSize);
            }

            TransformCube(cube1, newMatrix, bitmap);
            TransformCube(cube2, newMatrix, bitmap);

            Scene = ImageConverters.Bitmap2BitmapImage(bitmap);

            Image imageControl = (Image)this.FindName("SceneImage");
            imageControl.Source = Scene;
        }

        public void TransformCube(CustomCube cube, Matrix<double> matrix, Bitmap bitmap)
        {
            foreach (CustomTriangle triangle in cube.triangles)
            {
                List<CustomVertex> vertices = triangle.GetVertices();
                foreach (CustomVertex vertex in vertices)
                {
                    Vector<double> temp = TransformVertex(vertex, matrix);

                    vertex.x = temp[0];
                    vertex.y = temp[1];
                    vertex.z = temp[2];
                }
                bitmap = triangle.DrawTriangle(bitmap, Color.White);
            }
        }

        public Vector<double> TransformVertex(CustomVertex vertex, Matrix<double> newMatrix)
        {
            Vector<double> cords = Vector<double>.Build.DenseOfArray(new double[] { vertex.x, vertex.y, vertex.z, vertex.n });

            Vector<Double> temp = cords * newMatrix;
            temp = ((temp * M / temp[2] + C) / 2) * Scale;

            return temp;
        }
    }
}