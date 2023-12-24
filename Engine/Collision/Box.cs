using Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Collision
{
    public class Box
    {
        internal List<Point> PartitionCellPoints { get; } = new();

        public Box(Entity entity)
            : this(entity, 0f, 0f)
        {
        }

        public Box(Entity entity, float width, float height)
            : this(entity, width, height, 0)
        {
        }

        public Box(Entity entity, float width, float height, int layerMask) 
            : this(entity, 0f, 0f, width, height, layerMask)
        {
        }

        public Box(Entity entity, float x, float y, float width, float height)
            : this(entity, x, y, width, height, 0)
        {
        }

        public Box(Entity entity, float x, float y, float width, float height, int layerMask)
        {
            Entity = entity;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerMask = layerMask;
        }

        public event CollidedWithCallback CollidedWith;

        public Entity Entity
        {
            get;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        public int LayerMask
        {
            get;
            set;
        }

        public RectangleF CalculateBounds()
        {
            return Entity.Origin.Tranform(Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);
        }

        internal void NotifyCollidedWith(Box body, Collision collision)
        {
            CollidedWith?.Invoke(body, collision);
        }

        internal void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(CalculateBounds().ToXnaRect(), Color.White);
        }
    }
}
