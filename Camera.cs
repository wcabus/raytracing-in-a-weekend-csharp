using System;

namespace RayTracing
{
    public class Camera
    {
        private readonly Vector3 _origin;
        private readonly Vector3 _lowerLeftCorner;
        private readonly Vector3 _horizontal;
        private readonly Vector3 _vertical;
        private readonly double _lensRadius;
        private readonly Vector3 _w, _u, _v;

        public Camera(
            Vector3 lookFrom,
            Vector3 lookAt,
            Vector3 vup,
            double vFov,
            double aspectRatio,
            double aperture,
            double focusDistance
        )
        {
            var theta = vFov.ToRadians();
            var h = Math.Tan(theta / 2);
            var viewportHeight = 2 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            _w = Vector3.UnitVector(lookFrom - lookAt);
            _u = Vector3.UnitVector(Vector3.CrossProduct(vup, _w));
            _v = Vector3.CrossProduct(_w, _u);
            
            _origin = lookFrom;
            _horizontal = focusDistance * viewportWidth * _u;
            _vertical = focusDistance * viewportHeight * _v;
            _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - focusDistance * _w;
            _lensRadius = aperture / 2;
        }

        public Ray GetRay(double s, double t)
        {
            var rd = _lensRadius * Program.RandomInUnitDisc();
            var offset = _u * rd.X + _v * rd.Y;
            
            return new Ray(
                _origin + offset,
                _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset);
        }
    }
}