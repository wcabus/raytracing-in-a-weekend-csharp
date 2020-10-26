using System;
using RayTracing.Materials;

namespace RayTracing
{
    public class Sphere : Object3D
    {
        public Sphere(Vector3 center, double radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }
        
        public Vector3 Center { get; }
        public double Radius { get; }
        
        public override bool Hit(Ray r, double tMin, double tMax, out Object3D rec)
        {
            var oc = r.Origin - Center;
            var a = r.Direction.LengthSquared;
            var halfB = Vector3.DotProduct(oc, r.Direction);
            var c = oc.LengthSquared - Radius * Radius;
            var discriminant = halfB * halfB - a * c;

            if (discriminant > 0)
            {
                var root = Math.Sqrt(discriminant);
                var temp = (-halfB - root) / a;
                if (temp < tMax && temp > tMin)
                {
                    rec = new Sphere(Center, Radius, Material);
                    rec.T = temp;
                    rec.P = r.At(rec.T);
                    var outwardNormal = (rec.P - Center) / Radius;
                    rec.SetFaceNormal(r, outwardNormal);
                    return true;
                }

                temp = (-halfB + root) / a;
                if (temp < tMax && temp > tMin)
                {
                    rec = new Sphere(Center, Radius, Material);
                    rec.T = temp;
                    rec.P = r.At(rec.T);
                    var outwardNormal = (rec.P - Center) / Radius;
                    rec.SetFaceNormal(r, outwardNormal);
                    return true;
                }
            }

            rec = null;
            return false;
        }
    }
}