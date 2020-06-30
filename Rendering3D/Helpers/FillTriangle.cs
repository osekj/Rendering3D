using Rendering3D.Custom;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;

namespace Rendering3D.Helpers
{
    class FillTriangle
    {
        public static Bitmap Apply(Bitmap bitmap, List<CustomVertex> vertices, Color color)
        {
            Point[] points = new Point[3]
            {
                new Point((int)vertices[0].x, (int)vertices[0].y),
                new Point((int)vertices[1].x, (int)vertices[1].y),
                new Point((int)vertices[2].x, (int)vertices[2].y)
            };

            SolidBrush brush = new SolidBrush(color);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                graph.FillPolygon(brush, points);
            }

            return bitmap;
        }
    }
}
