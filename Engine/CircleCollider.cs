using Microsoft.Xna.Framework;

namespace Engine
{
    public class CircleCollider : Collider
    {
        public CircleCollider(Entity entity, Vector2 position, float radius)
            : this(entity, position, radius, 0)
        {
        }

        public CircleCollider(Entity entity, Vector2 position, float radius, int layerMask)
            : base(entity)
        {
            Position = position;
            Radius = radius;
            LayerMask = layerMask;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public float Radius
        {
            get;
        }

        public CircleF Circle
        {
            get => new(Bounds.Center, Radius);
        }

        public override RectangleF Bounds
        {
            get => Entity.Origin.Tranform(Entity.Position.X + Position.X, Entity.Position.Y + Position.Y, Radius * 2f, Radius * 2f);
        }

        public override void MoveToPosition(Vector2 position)
        {
            Entity.Position = position - Position;
            UpdateBBox();
        }

        public override bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return IntersectionTests.MovingCircleVsRectangle(Circle, path, collider.Bounds, out time, out contact, out normal);
        }

        public override bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return IntersectionTests.MovingCircleVsCircle(Circle, path, collider.Circle, out time, out contact, out normal);
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawCircleOutline(Entity.Position.X - Position.X, Entity.Position.Y + Position.Y, Radius, 10, new Color(Color.White, 0.1f));
        }
    }
}
