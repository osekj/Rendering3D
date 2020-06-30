using System;

namespace Rendering3D.Converters
{
    class DegreesToRadians
    {
        public static double Convert(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
