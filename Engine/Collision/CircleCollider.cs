using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    public class CircleCollider : Collider
    {
        public CircleCollider(Entity entity, Vector2 position, float radius)
            : base(entity)
        {
            Position = position;
            Radius = radius;
        }

        public CircleCollider(Entity entity, Vector2 position, float radius, int layerMask)
            : this(entity, position, radius)
        {
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

        public override bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return IntersectionTests.MovingCircleVsRectangle(Circle, path, collider.Bounds, out time, out contact, out normal);
        }

        public override bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return IntersectionTests.MovingCircleVsCircle(Circle, path, collider.Circle, out time, out contact, out normal);
        }

        public override void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawCircleOutline(Entity.Position.X - Position.X, Entity.Position.Y + Position.Y, Radius, 10, new Color(Color.White, 0.1f));
        }
    }
}
