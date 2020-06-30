namespace Rendering3D.Custom
{
    public class CustomVertex
    {
        public double x;
        public double y;
        public double z;
        public double n;

        public CustomVertex(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.n = 1;
        }
    }
}
