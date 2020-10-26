using System.Collections.Generic;

namespace RayTracing
{
    public static class EnumerableExtensions
    {
        public static bool Hit(this IEnumerable<Object3D> source, Ray ray, double min, double max, out Object3D hit)
        {
            hit = null;
            
            var hitDetected = false;
            var closestSoFar = max;
            foreach (var item in source)
            {
                if (!item.Hit(ray, min, closestSoFar, out var localHit))
                {
                    continue;
                }

                hit = localHit;
                hitDetected = true;
                closestSoFar = localHit.T;
            }

            return hitDetected;
        }
    }
}