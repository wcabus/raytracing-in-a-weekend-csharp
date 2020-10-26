using System;

namespace RayTracing
{
    public static class DoubleExtensions
    {
        public static double ToRadians(this double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}