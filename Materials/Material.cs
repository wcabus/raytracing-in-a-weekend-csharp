using System;

namespace RayTracing.Materials
{
    public abstract class Material
    {
        public virtual bool Scatter(Ray ray, Object3D rec, out Vector3 attenuation, out Ray scattered)
        {
            attenuation = default;
            scattered = default;
            return false;
        }
        
        protected Vector3 Reflect(Vector3 v, Vector3 n)
        {
            return v - 2 * Vector3.DotProduct(v, n) * n;
        }
        
        public Vector3 Refract(Vector3 uv, Vector3 normal, double etaiOverEtat)
        {
            var cosTheta = Vector3.DotProduct(-uv, normal);
            var rOutPerp = etaiOverEtat * (uv + cosTheta * normal);
            var rOutParallel = -Math.Sqrt(Math.Abs(1.0 - rOutPerp.LengthSquared)) * normal;

            return rOutPerp + rOutParallel;
        }
    }
}