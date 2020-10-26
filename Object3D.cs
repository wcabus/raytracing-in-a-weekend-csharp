using RayTracing.Materials;

namespace RayTracing
{
    public abstract class Object3D
    {
        public Vector3 P { get; protected internal set; }
        public Vector3 Normal { get; private set; }
        public double T { get; protected internal set; }
        private bool FrontFace { get; set; }
        
        public Material Material { get; protected set; }

        public abstract bool Hit(Ray r, double tMin, double tMax, out Object3D rec);

        public void SetFaceNormal(Ray r, Vector3 outwardNormal)
        {
            FrontFace = Vector3.DotProduct(r.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}