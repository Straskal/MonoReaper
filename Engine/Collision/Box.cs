using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public class Box : Component
    {
        /// <summary>
        /// The partition cell points that contain this box.
        /// These are cached on the box itself to avoid unnecessary lookups in the partition.
        /// </summary>
        internal List<Point> PartitionCellPoints { get; } = new();

        public Box(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Box(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Box(float x, float y, float width, float height, int layerMask)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerMask = layerMask;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int LayerMask { get; set; }

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
