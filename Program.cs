using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using RayTracing.Materials;

namespace RayTracing
{
    public static class Program
    {
        private const double AspectRatio = 3.0 / 2;
        private const int Width = 1200;
        private const int Height = (int)(Width / AspectRatio);
        private const int SamplesPerPixel = 500;
        private const int MaxDepth = 50;

        private static Camera _camera;

        private static readonly List<Object3D> World = new List<Object3D>();
        
        public static void Main()
        {
            RandomizeScene();

            var lookFrom = new Vector3(13, 2, 3);
            var lookAt = new Vector3(0, 0, 0);
            var vup = new Vector3(0, 1, 0);
            var aperture = 0.1;
            var distToFocus = 10.0;
            
            _camera = new Camera(
                lookFrom,
                lookAt,
                vup,
                20.0,
                AspectRatio,
                aperture,
                distToFocus);
            
            Render();
        }

        private static void RandomizeScene()
        {
            var groundMaterial = new Lambertian(new Vector3(0.5, 0.5, 0.5));
            World.Add(new Sphere(new Vector3(0,-1000,0), 1000, groundMaterial));

            var offset = new Vector3(4, 0.2, 0);
            for (var a = -11; a < 11; a++)
            {
                for (var b = -11; b < 11; b++)
                {
                    var chooseMat = RandomDouble();
                    var center = new Vector3(a + 0.9 * RandomDouble(), 0.2, b + 0.9 * RandomDouble());

                    if ((center - offset).Length > 0.9)
                    {
                        Material material;
                        switch (chooseMat)
                        {
                            case { } w when w < 0.8:
                                // diffuse
                                var color = Vector3.Random() * Vector3.Random();
                                material = new Lambertian(color);
                                break;
                            case { } w2 when w2 < 0.95:
                                // metal
                                var color2 = Vector3.Random(0.5, 1);
                                var fuzz = RandomDouble(0, 0.5);
                                material = new Metal(color2, fuzz);
                                break;
                            default:
                                // glass
                                material = new Dielectric(1.5);
                                break;
                        }
                        
                        World.Add(new Sphere(center, 0.2, material));
                    }
                }
            }
            
            var material1 = new Dielectric(1.5);
            World.Add(new Sphere(new Vector3(0, 1, 0), 1.0, material1));
            
            var material2 = new Lambertian(new Vector3(0.4, 0.2, 0.1));
            World.Add(new Sphere(new Vector3(-4, 1, 0), 1.0, material2));
            
            var material3 = new Metal(new Vector3(0.7, 0.6, 0.5), 0);
            World.Add(new Sphere(new Vector3(4, 1, 0), 1.0, material3));
        }

        private static void Render()
        {
            var timer = Stopwatch.StartNew();
            using var output = new Bitmap(Width, Height);
            using var g = Graphics.FromImage(output);
            
            var targets = new List<Rectangle>();
            for (var y = 0; y < Height; y += 100)
            {
                for (var x = 0; x < Width; x += 100)
                {
                    targets.Add(new Rectangle(x, y, 100, 100));
                }
            }

            Console.Clear();
            var cursorTop = 0;
            Parallel.For(0, targets.Count, i =>
            {
                var top = Interlocked.Increment(ref cursorTop) - 1;
                lock (Console.Out)
                {
                    Console.SetCursorPosition(0, top);
                    Console.WriteLine($"Rendering target {i + 1}...");
                }

                var target = targets[i];
                using var targetBuffer = new Bitmap(target.Width, target.Height);
                RenderArea(target, targetBuffer);

                lock (Console.Out)
                {
                    Console.SetCursorPosition(0, top);
                    Console.WriteLine($"Rendering target {i+1}...Done!");
                }
                
                lock (g)
                {
                    g.DrawImage(targetBuffer, target.Location);
                }
            });

            g.Flush();
            output.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            timer.Stop();
            Console.WriteLine();
            Console.WriteLine($"Done in {timer.Elapsed}.");
            
            output.Save(@"C:\temp\render.bmp", ImageFormat.Bmp);
        }
        
        private static void RenderArea(Rectangle target, Bitmap buffer)
        {
            for (var y = target.Height - 1; y >= 0; --y)
            {
                for (var x = 0; x < target.Width; x++)
                {
                    var pixel = Vector3.Zero;
                    for (var s = 0; s < SamplesPerPixel; ++s)
                    {
                        var tX = x + target.Left;
                        var tY = y + target.Top;
                        var u = (tX + RandomDouble()) / (Width - 1.0);
                        var v = (tY + RandomDouble()) / (Height - 1.0);
                        var ray = _camera.GetRay(u, v);
                        pixel += RayColor(ray, World, MaxDepth);
                    }

                    buffer.SetPixel(x, y, pixel.ToColor(SamplesPerPixel));
                }
            }
        }

        public static Vector3 RandomInUnitSphere()
        {
            var p = Vector3.Random(-1, 1);
            while (true)
            {
                if (p.LengthSquared >= 1) {
                    p.X = RandomDouble(-1, 1);
                    p.Y = RandomDouble(-1, 1);
                    p.Z = RandomDouble(-1, 1);
                    continue;
                }

                return p;
            }
        }
        
        public static Vector3 RandomInUnitDisc()
        {
            var p = new Vector3(RandomDouble(-1, 1), RandomDouble(-1, 1), 0);
            while (true)
            {
                if (p.LengthSquared >= 1)
                {
                    p.X = RandomDouble(-1, 1);
                    p.Y = RandomDouble(-1, 1);
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
                if (rec.Material.Scatter(r, rec, out var attenuation, out var scattered))
                {
                    return attenuation * RayColor(scattered, world, depth - 1);
                }
                
                return new Vector3(0, 0, 0);
            }

            var unitDirection = Vector3.UnitVector(r.Direction);
            var t = 0.5 * unitDirection.Y + 1.0;
            return (1.0 - t) * new Vector3(1, 1, 1) + t * new Vector3(.5, .7, 1);
        }

        public static double RandomDouble() => StaticRandom.NextDouble();
        
        public static double RandomDouble(double min, double max) =>
            min + (max - min) * RandomDouble();
    }
}
