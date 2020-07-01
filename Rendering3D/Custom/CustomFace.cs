using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rendering3D.Custom
{
    public class CustomFace
    {
        public CustomTriangle triangleA;
        public CustomTriangle triangleB;

        public double avgZ;

        public Color color;

        public CustomFace(CustomTriangle triangleA, CustomTriangle triangleB)
        {
            this.triangleA = triangleA;
            this.triangleB = triangleB;

            Random random = new Random(Guid.NewGuid().GetHashCode());

            int r = random.Next(0, 255);
            int g = random.Next(0, 255);
            int b = random.Next(0, 255);

            Color colorToApply = Color.FromArgb(r, g, b);

            this.color = colorToApply;

            UpdateAverageZ();
        }

        public void UdateTriangles(CustomTriangle triangleA, CustomTriangle triangleB)
        {
            this.triangleA = triangleA;
            this.triangleB = triangleB;
        }

        public Tuple<CustomTriangle, CustomTriangle> GetTriangles()
        {
            return Tuple.Create(triangleA, triangleB);
        }

        public void UpdateAverageZ()
        {
            List<CustomVertex> vertices = new List<CustomVertex>();
            vertices.AddRange(this.triangleA.vertices);
            vertices.AddRange(this.triangleB.vertices);

            double sum = 0;
            foreach (CustomVertex vertex in vertices)
                sum += vertex.z;

            avgZ = sum / vertices.Count;
        }
    }
}
