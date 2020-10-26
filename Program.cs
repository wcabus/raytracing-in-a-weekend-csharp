// https://raytracing.github.io/books/RayTracingInOneWeekend.html
// 10. Dielectrics

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RayTracing.Materials;

namespace RayTracing
{
    public static class Program
    {
        private const double AspectRatio = 16 / 9.0;
        private const int Width = 400;
        private const int Height = (int)(Width / AspectRatio);
        private const int SamplesPerPixel = 100;
        private const int MaxDepth = 50;

        private static readonly Camera Camera = new Camera();

        private static readonly List<Object3D> World = new List<Object3D>();
        
        public static async Task Main()
        {
            var groundMat = new Lambertian(new Vector3(0.8, 0.8, 0));
            var centerMat = new Lambertian(new Vector3(0.7, 0.3, 0.3));
            var leftMat = new Metal(new Vector3(0.8, 0.8, 0.8), 0.3);
            var rightMat = new Metal(new Vector3(0.8, 0.6, 0.2), 1.0);
            
            World.Add(new Sphere(new Vector3(0, -100.5, -1), 100, groundMat));
            World.Add(new Sphere(new Vector3(0, 0, -1), .5, centerMat));
            World.Add(new Sphere(new Vector3(-1, 0, -1), .5, leftMat));
            World.Add(new Sphere(new Vector3(1, 0, -1), .5, rightMat));
            
            await Render();
        }

        private static async Task Render()
        {
            var timer = Stopwatch.StartNew();
            var encoding = new UTF8Encoding(false);
            await using var sw = new StreamWriter(@"C:\Temp\render.ppm", false, encoding);
            await sw.WriteLineAsync("P3");
            await sw.WriteLineAsync($"{Width} {Height}");
            await sw.WriteLineAsync("255");

            var cursorY = Console.CursorTop;
            var scanline = ArrayPool<Vector3>.Shared.Rent(Width);
            
            for (var j = Height - 1; j >= 0; --j)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = cursorY;
                Console.WriteLine($"Scanlines remaining: {j}  ");
             
                // for (var i = 0; i < Width; i++)
                Parallel.For(0, Width, i =>
                    {
                        var pixel = Vector3.Zero;
                        for (var s = 0; s < SamplesPerPixel; ++s)
                        {
                            var u = (i + RandomDouble()) / (Width - 1.0);
                            var v = (j + RandomDouble()) / (Height - 1.0);
                            var ray = Camera.GetRay(u, v);
                            pixel += RayColor(ray, World, MaxDepth);
                        }

                        scanline[i] = pixel;
                    });
                // }
                
                for (var i = 0; i < Width; i++)
                {
                    await scanline[i].WriteColor(sw, SamplesPerPixel);
                }
            }
            
            ArrayPool<Vector3>.Shared.Return(scanline);
            timer.Stop();
            Console.WriteLine($"Done in {timer.Elapsed}.");
        }

        public static Vector3 RandomInUnitSphere()
        {
            while (true)
            {
                var p = Vector3.Random(-1, 1);
                if (p.LengthSquared >= 1){
                    continue;
                }

                return p;
            }
        }
        
        public static Vector3 RandomInHemisphere(Vector3 normal)
        {
            var inUnitSphere = RandomInUnitSphere();
            if (Vector3.DotProduct(inUnitSphere, normal) > 0.0)
            {
                return inUnitSphere;
            }

            return -inUnitSphere;
        }

        public static Vector3 RandomUnitVector()
        {
            var a = RandomDouble(0, 2 * Math.PI);
            var z = RandomDouble(-1, 1);
            var r = Math.Sqrt(1 - z * z);
            return new Vector3(r * Math.Cos(a), r * Math.Sin(a), z);
        }
        
        private static Vector3 RayColor(Ray r, IEnumerable<Object3D> world, int depth)
        {
            if (depth <= 0)
            {
                // we went to far without any hits, return blackness
                return new Vector3(0, 0, 0);
            }
            
            if (world.Hit(r, 0.001, double.PositiveInfinity, out var rec))
            {
                Ray scattered;
                Vector3 attenuation;
                if (rec.Material.Scatter(r, rec, out attenuation, out scattered))
                {
                    return attenuation * RayColor(scattered, world, depth - 1);
                }
                
                return new Vector3(0, 0, 0);
            }

            var unitDirection = r.Direction.ToUnit();
            var t = 0.5 * unitDirection.Y + 1.0;
            return (1.0 - t) * new Vector3(1, 1, 1) + t * new Vector3(.5, .7, 1);
        }

        public static double RandomDouble() => StaticRandom.NextDouble();
        
        public static double RandomDouble(double min, double max) =>
            min + (max - min) * RandomDouble();
    }
}
