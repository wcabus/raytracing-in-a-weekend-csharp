using System;
using System.Threading;

namespace RayTracing
{
    public static class StaticRandom
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random());

        public static double NextDouble()
        {
            return Random.Value.NextDouble();
        }
    }
}