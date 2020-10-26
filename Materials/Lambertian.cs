using System;

namespace RayTracing.Materials
{
    public class Lambertian : Material
    {
        public Lambertian(Vector3 color)
        {
            Color = color;
        }

        public Vector3 Color { get; }

        public override bool Scatter(Ray ray, Object3D rec, out Vector3 attenuation, out Ray scattered)
        {
            var scatterDirection = rec.Normal + Program.RandomUnitVector();
            scattered = new Ray(rec.P, scatterDirection);
            attenuation = Color;
            return true;
        }
    }
}