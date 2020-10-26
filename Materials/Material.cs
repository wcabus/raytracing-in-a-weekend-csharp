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
    }
}