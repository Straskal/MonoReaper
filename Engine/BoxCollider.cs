using Microsoft.Xna.Framework;

namespace Engine
{
    public class BoxCollider : Collider
    {
        public BoxCollider(Entity entity)
            : this(entity, 0f, 0f)
        {
        }

        public BoxCollider(Entity entity, float width, float height)
            : this(entity, width, height, 0)
        {
        }

        public BoxCollider(Entity entity, float width, float height, int layerMask)
            : this(entity, 0f, 0f, width, height, layerMask)
        {
        }

        public BoxCollider(Entity entity, float x, float y, float width, float height)
            : this(entity, x, y, width, height, 0)
        {
        }

        public BoxCollider(Entity entity, float x, float y, float width, float height, int layerMask)
            : base(entity)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Layer = layerMask;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override RectangleF Bounds => Entity.Origin.Tranform(Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);

        public override bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return IntersectionTests.MovingRectangleVsRectangle(Bounds, path, collider.Bounds, out time, out contact, out normal);
        }

        public override bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            // TODO: Make moving rectangle vs circle intersections work correctly. Right now they are treated as tectangle vs rectangle.
            return IntersectionTests.MovingRectangleVsRectangle(Bounds, path, collider.Bounds, out time, out contact, out normal);
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.White);
        }
    }
}
