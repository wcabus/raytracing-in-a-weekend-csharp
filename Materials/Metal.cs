namespace RayTracing.Materials
{
    public class Metal : Material
    {
        public Metal(Vector3 color, double fuzz)
        {
            Color = color;
            Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public Vector3 Color { get; }
        public double Fuzz { get; }

        public override bool Scatter(Ray ray, Object3D rec, out Vector3 attenuation, out Ray scattered)
        {
            var reflected = Reflect(ray.Direction.ToUnit(), rec.Normal);
            scattered = new Ray(rec.P, reflected + Fuzz * Program.RandomInUnitSphere());
            attenuation = Color;
            return Vector3.DotProduct(scattered.Direction, rec.Normal) > 0;
        }
    }
}