using System;

namespace RayTracing.Materials
{
    public class Dielectric : Material
    {
        public Dielectric(double refractionIndex)
        {
            RefractionIndex = refractionIndex;
        }
        
        public double RefractionIndex { get; }

        public override bool Scatter(Ray ray, Object3D rec, out Vector3 attenuation, out Ray scattered)
        {
            attenuation = new Vector3(1,1,1);
            var refractionRatio = rec.FrontFace ? 1.0 / RefractionIndex : RefractionIndex;

            var unitDirection = Vector3.UnitVector(ray.Direction);
            var cosTheta = Math.Min(Vector3.DotProduct(-unitDirection, rec.Normal), 1.0);
            var sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            var cantRefract = refractionRatio * sinTheta > 1.0;

            var direction = cantRefract || Reflectance(cosTheta, refractionRatio) > Program.RandomDouble() ?
                Reflect(unitDirection, rec.Normal) :
                Refract(unitDirection, rec.Normal, refractionRatio);
            
            scattered = new Ray(rec.P, direction);
            return true;
        }

        private double Reflectance(double cosine, double refractionIndex)
        {
            // Use Schlick's approximation for reflectance.
            var r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 *= r0;

            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}