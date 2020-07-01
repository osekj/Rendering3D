using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace Rendering3D.Custom
{
    public class CustomCube
    {
        private double[] position;

        public List<CustomVertex> vertices;
        public List<CustomTriangle> triangles;
        public List<CustomFace> faces;

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
            faces = new List<CustomFace>();

            // Front
            triangles.Add(new CustomTriangle(vertices[0], vertices[1], vertices[2]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[2], vertices[3]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));

            // Top
            triangles.Add(new CustomTriangle(vertices[1], vertices[3], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[5], vertices[7]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));

            // Back
            triangles.Add(new CustomTriangle(vertices[4], vertices[5], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[4], vertices[6], vertices[7]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));

            // Bottom
            triangles.Add(new CustomTriangle(vertices[0], vertices[2], vertices[4]));
            triangles.Add(new CustomTriangle(vertices[2], vertices[4], vertices[6]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));

            // Left
            triangles.Add(new CustomTriangle(vertices[0], vertices[1], vertices[4]));
            triangles.Add(new CustomTriangle(vertices[1], vertices[4], vertices[5]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));

            // Right
            triangles.Add(new CustomTriangle(vertices[2], vertices[3], vertices[7]));
            triangles.Add(new CustomTriangle(vertices[2], vertices[6], vertices[7]));
            faces.Add(new CustomFace(triangles[triangles.Count - 1], triangles[triangles.Count - 2]));
        }

        public void UdateFacesTriangles()
        {
            int j = 0;
            for(int i = 0; i < triangles.Count; i += 2)
            {
                faces[j].triangleA = triangles[i];
                faces[j].triangleB = triangles[i + 1];
                faces[j].UpdateAverageZ();
                ++j;
            }
        }

        public double GetArea()
        {
            return 6 * (a * a);
        }

        public double GetCenterY()
        {
            return a / 2;
        }

        public Bitmap Draw(Bitmap bitmap)
        {
            this.UdateFacesTriangles();

            List<CustomFace> sortedFaces = faces.OrderByDescending(o => o.avgZ).ToList();

            foreach (CustomFace face in sortedFaces)
            {
                Tuple<CustomTriangle, CustomTriangle> faceTriangles = face.GetTriangles();
                
                bitmap = Helpers.FillTriangle.Apply(bitmap, faceTriangles.Item1.vertices, face.color);
                bitmap = Helpers.FillTriangle.Apply(bitmap, faceTriangles.Item2.vertices, face.color);
            }

            return bitmap;
        }


    }
}
