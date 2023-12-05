using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Core.Collision
{
    public class Box : Component
    {
        internal List<Point> Points { get; } = new();

        public Box(float x, float y, float width, float height) : this(x, y, width, height, 0)
        {
        }

        public Box(float x, float y, float width, float height, int layerMask)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerMask = layerMask;
        }

        public int LayerMask { get; set; }
        public Vector2 Size { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override void OnSpawn()
        {
            Level.Partition.Add(this);
        }

        public override void OnDestroy()
        {
            Level.Partition.Remove(this);
        }

        public RectangleF CalculateBounds()
        {
            return Offset.GetRect(Entity.Origin, Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);
        }

        public void MoveTo(Vector2 position)
        {
            Entity.Position = position;
            UpdateBBox();
        }

        public void UpdateBBox()
        {
            Level.Partition.Update(this);
        }
    }
}
