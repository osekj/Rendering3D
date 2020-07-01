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

        double theta = Math.PI / 2;

        double near = 0.1;
        double far = 1000;

        Matrix<double> M;
        Matrix<double> Scale;
        Matrix<double> Translate;
        Matrix<double> Zoom;

        Vector<double> Camera = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0 });

        Vector<double> Shift = DenseVector.Build.DenseOfArray(new double[4] { 1, 0.5, 0, 0 });

        double rotationChange = Math.PI / 300;

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
            Scale[0, 0] = 300;
            Scale[1, 1] = 300;

            Translate = DenseMatrix.CreateIdentity(4);
             
            Zoom = DenseMatrix.CreateIdentity(4);
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
            cube2 = new CustomCube(550, 300, 10, 50);

            bitmap = cube1.Draw(bitmap);
            bitmap = cube2.Draw(bitmap);

            Scene = ImageConverters.Bitmap2BitmapImage(bitmap);

            imageControl.Source = Scene;

            double tanValue = 1 / Math.Tan(theta / 2);

            //T[3, 1] = cube1.GetCenterY();

            //Zoom[3, 1] = cube1.GetCenterY();
            Zoom[3, 2] = 5;
            ZoomProperty = 30;

            M = DenseMatrix.OfArray(new double[,] { { tanValue, 0, 0, 0 },
                                                    { 0, tanValue, 0, 0 },
                                                    { 0, 0, -far/(far-near), -1 },
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
                xRotation += rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                xRotation -= rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                yRotation -= rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                yRotation += rotationChange;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.Q)
            {
                Zoom[3, 2]--;
                wasControlKeyDown = true;
            }
            if (e.Key == Key.E)
            {
                Zoom[3, 2]++;
                wasControlKeyDown = true;
            }

            if (wasControlKeyDown)
                Redraw();
        }

        #endregion

        private void Redraw()
        {
            Bitmap bitmap = ImageConverters.BitmapImage2Bitmap(Scene);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, sceneWidth, sceneHeight);
                graph.FillRectangle(Brushes.Black, ImageSize);
            }

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

            Matrix<double> finalMatrix = DenseMatrix.CreateIdentity(4) * Translate * rotationY * rotationX * rotationZ * Zoom;

            bitmap = TransformCube(cube1, finalMatrix, bitmap);
            bitmap = TransformCube(cube2, finalMatrix, bitmap);

            Scene = ImageConverters.Bitmap2BitmapImage(bitmap);

            Image imageControl = (Image)this.FindName("SceneImage");
            imageControl.Source = Scene;
        }

        public Bitmap TransformCube(CustomCube cube, Matrix<double> matrix, Bitmap bitmap)
        {
            List<CustomTriangle> newTriangles = new List<CustomTriangle>();
            foreach (CustomTriangle triangle in cube.triangles)
            {
                List<Vector<double>> points = new List<Vector<double>>();
                foreach (CustomVertex vertex in triangle.vertices)
                {
                    Vector<double> temp = Vector<double>.Build.DenseOfArray(new double[] { vertex.x, vertex.y, vertex.z, 1 });
                    Vector<double> point = temp * matrix;
                    points.Add(point);
                }

                Vector<double> normal = CrossProduct(points[1] - points[0], points[2] - points[0]);

                Vector<double> normalTemp = Vector<double>.Build.DenseOfArray(
                                                new double[] { Math.Pow(normal[0], 3), Math.Pow(normal[1], 3), Math.Pow(normal[2], 3) });
                
                normal = normal / Math.Sqrt(normalTemp.Sum());

                foreach (Vector<double> point in points)
                {
                    Vector<double> temp = (point * matrix / point[2] + Shift) / 2;
                }

                CustomTriangle newTriangle = new CustomTriangle(
                                                    new CustomVertex(points[0][0], points[0][1], points[0][2]),
                                                    new CustomVertex(points[1][0], points[1][1], points[1][2]),
                                                    new CustomVertex(points[2][0], points[2][1], points[2][2])
                                                );
                newTriangles.Add(newTriangle);
            }

            List<CustomVertex> newVertices = new List<CustomVertex>();
            foreach(CustomTriangle triangle in newTriangles)
            {
                newVertices.AddRange(triangle.vertices);
            }

            cube.triangles = newTriangles;
            cube.vertices = newVertices;
            bitmap = cube.Draw(bitmap);

            return bitmap;
        }

        //public Vector<double> TransformVertex(CustomVertex vertex, Matrix<double> newMatrix)
        //{
        //    Vector<double> cords = Vector<double>.Build.DenseOfArray(new double[] { vertex.x, vertex.y, vertex.z, vertex.n });

        //    Vector<Double> temp = cords * newMatrix;
        //    temp = ((temp * M / temp[2] + C) / 2) * Scale;

        //    return temp;
        //}

        public Vector<double> CrossProduct(Vector<double> v, Vector<double> w)
        {
            Vector<double> vec = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
            vec[0] = v[1] * w[2] - v[2] * w[1];
            vec[1] = v[2] * w[0] - v[0] * w[2];
            vec[2] = v[0] * w[1] - v[1] * w[0];
            return vec;
        }
    }
}