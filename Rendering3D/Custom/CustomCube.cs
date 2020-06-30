using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace Rendering3D.Custom
{
    public class CustomCube
    {
        private double[] position;

        public List<CustomVertex> vertices;
        public List<CustomTriangle> triangles;

        private double a;

        public CustomCube(double x, int y, double z, double a)
        {
            position = new double[3] { x, y, z };

            this.a = a;

            GetVertices();
            GetTriangles();
        }

        public void GetVertices()
        {
            vertices = new List<CustomVertex>();

            vertices.Add(new CustomVertex(position[0] - a / 2, position[1] - a / 2, position[2] - a / 2));
            vertices.Add(new CustomVertex(position[0] - a / 2, position[1] + a / 2, position[2] - a / 2));
            vertices.Add(new CustomVertex(position[0] + a / 2, position[1] - a / 2, position[2] - a / 2));
            vertices.Add(new CustomVertex(position[0] + a / 2, position[1] + a / 2, position[2] - a / 2));
            vertices.Add(new CustomVertex(position[0] - a / 2, position[1] - a / 2, position[2] + a / 2));
            vertices.Add(new CustomVertex(position[0] - a / 2, position[1] + a / 2, position[2] + a / 2));
            vertices.Add(new CustomVertex(position[0] + a / 2, position[1] - a / 2, position[2] + a / 2));
            vertices.Add(new CustomVertex(position[0] + a / 2, position[1] + a / 2, position[2] + a / 2));
        }

        public void GetTriangles()
        {
            triangles = new List<CustomTriangle>();

            // Front
            triangles.Add(new CustomTriangle(vertices[0], vertices[1], vertices[2]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[2], vertices[3]));

            // Top
            triangles.Add(new CustomTriangle(vertices[1], vertices[3], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[5], vertices[7]));

            // Back
            triangles.Add(new CustomTriangle(vertices[4], vertices[5], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[4], vertices[6], vertices[7]));

            // Bottom
            triangles.Add(new CustomTriangle(vertices[0], vertices[2], vertices[4]));
            triangles.Add(new CustomTriangle(vertices[2], vertices[4], vertices[6]));

            // Left
            triangles.Add(new CustomTriangle(vertices[0], vertices[1], vertices[4]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[4], vertices[5]));

            // Right
            triangles.Add(new CustomTriangle(vertices[2], vertices[3], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[2], vertices[6], vertices[7]));
        }

        public double GetArea()
        {
            return 6 * (a * a);
        }

        public Bitmap Draw(Bitmap bitmap)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            int r = random.Next(0, 255);
            int g = random.Next(0, 255);
            int b = random.Next(0, 255);

            Color color = Color.FromArgb(r, g, b);

            foreach (CustomTriangle triangle in triangles)
            {
                try
                {
                    bitmap = triangle.DrawTriangle(bitmap, color);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }            

            return bitmap;
        }
    }
}
