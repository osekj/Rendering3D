using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Drawing;

namespace Rendering3D.Custom
{
    public class CustomTriangle
    {
        public List<CustomVertex> vertices;

        public CustomTriangle(CustomVertex a, CustomVertex b, CustomVertex c)
        {
            vertices = new List<CustomVertex> { a, b, c };
        }

        public List<CustomVertex> GetVertices()
        {
            List<CustomVertex> verticesToReturn = new List<CustomVertex>(vertices);
            return verticesToReturn;
        }

        public Bitmap DrawTriangle(Bitmap bitmap)
        {
            Pen pen = new Pen(Color.White, 1);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                
                graphics.DrawLine(pen, (float)vertices[0].x, (float)vertices[0].y,
                                    (float)vertices[1].x, (float)vertices[1].y);

                graphics.DrawLine(pen, (float)vertices[0].x, (float)vertices[0].y,
                                    (float)vertices[2].x, (float)vertices[2].y);

                graphics.DrawLine(pen, (float)vertices[1].x, (float)vertices[1].y,
                                    (float)vertices[2].x, (float)vertices[2].y);
            }

            return bitmap;
        }
    }
}
