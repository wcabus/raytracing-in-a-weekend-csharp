using System;
using System.IO;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Vector3
    {
        public static Vector3 Zero => new Vector3();
        
        private Vector3() : this(0,0,0) {}
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public double Length => Math.Sqrt(LengthSquared);
        public double LengthSquared => X * X + Y * Y + Z * Z;

        public void Reset()
        {
            X = Y = Z = 0;
        }
        
        public Vector3 ToUnit() => this / Length;

        public Task Write(TextWriter tw)
        {
            return tw.WriteLineAsync($"{X} {Y} {Z}");
        }
        
        public Task WriteColor(TextWriter tw, int samplesPerPixel)
        {
            var r = X;
            var g = Y;
            var b = Z;
            var scale = 1.0 / samplesPerPixel;

            r = Math.Sqrt(scale * r);
            g = Math.Sqrt(scale * g);
            b = Math.Sqrt(scale * b);

            r = 256 * Clamp(r, 0.0, 0.999);
            g = 256 * Clamp(g, 0.0, 0.999);
            b = 256 * Clamp(b, 0.0, 0.999);
            
            return tw.WriteLineAsync($"{(int)r} {(int)g} {(int)b}");
        }

        private double Clamp(double x, double min, double max)
        {
            if (x < min) return min;
            return x > max ? max : x;
        }

        public static double DotProduct(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        
        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X + a.X * b.Z,
                a.X * b.Y + a.Y * b.X
            );
        }

        public static Vector3 Random()
        {
            return new Vector3(Program.RandomDouble(), Program.RandomDouble(), Program.RandomDouble());
        }
        
        public static Vector3 Random(double min, double max)
        {
            return new Vector3(Program.RandomDouble(min, max), Program.RandomDouble(min, max), Program.RandomDouble(min, max));
        }
        
        public static Vector3 operator -(Vector3 v) => new Vector3(-v.X, -v.Y, -v.Z);
        
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(
            a.X + b.X,
            a.Y + b.Y,
            a.Z + b.Z);
        
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(
            a.X - b.X,
            a.Y - b.Y,
            a.Z - b.Z);
        
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(
            a.X * b.X,
            a.Y * b.Y,
            a.Z * b.Z);
        
        public static Vector3 operator *(Vector3 a, double b) => new Vector3(
            a.X * b,
            a.Y * b,
            a.Z * b);
        
        public static Vector3 operator *(double b, Vector3 a) => new Vector3(
            a.X * b,
            a.Y * b,
            a.Z * b);
        
        public static Vector3 operator /(Vector3 a, double b) => new Vector3(
            a.X / b,
            a.Y / b,
            a.Z / b);
    }
}