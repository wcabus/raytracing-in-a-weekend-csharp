namespace RayTracing
{
    public class Camera
    {
        private readonly Vector3 _origin;
        private readonly Vector3 _lowerLeftCorner;
        private readonly Vector3 _horizontal;
        private readonly Vector3 _vertical;
        
        public Camera()
        {
            AspectRatio = 16.0 / 9;
            ViewportHeight = 2;
            ViewportWidth = AspectRatio * ViewportHeight;
            FocalLength = 1;

            _origin = Vector3.Zero;
            _horizontal = new Vector3(ViewportWidth, 0, 0);
            _vertical = new Vector3(0, ViewportHeight, 0);
            _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - new Vector3(0, 0, FocalLength);
        }

        public Ray GetRay(double u, double v)
        {
            return new Ray(_origin, _lowerLeftCorner + u * _horizontal + v * _vertical - _origin);
        }
        
        public double AspectRatio { get; }
        public double ViewportHeight { get; }
        public double ViewportWidth { get; }
        public double FocalLength { get; }
        
    }
}