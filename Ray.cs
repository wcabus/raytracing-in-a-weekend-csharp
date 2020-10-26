namespace RayTracing
{
    public readonly struct Ray
    {
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
        
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Vector3 At(double t) => Origin + t * Direction;
    }
}