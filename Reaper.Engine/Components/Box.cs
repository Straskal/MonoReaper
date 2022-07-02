﻿using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;

namespace Reaper.Engine.Components
{
    public class Box : Component
    {
        public Box(CollisionLayer type, float width, float height)
        {
            Layer = type;
            Width = width;
            Height = height;
        }

        public CollisionLayer Layer { get; set; }
        public bool IsSolid => Layer == CollisionLayer.Solid;

        public RectangleF Bounds => OriginHelpers.GetOffsetRect(
            Entity.Origin,
            Entity.Position.X,
            Entity.Position.Y,
            Width,
            Height);

        public Vector2 Size { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override void OnSpawn()
        {
            Level.Partition.Add(this);
        }

        public override void OnDestroy()
        {
            Level.Partition.Remove(this, Entity.Position);
        }

        public override void OnDetach()
        {
            Level.Partition.Remove(this, Entity.Position);
        }

        public void Move(Vector2 direction)
        {
            var previousPosition = Entity.Position;

            Entity.Position += direction;

            UpdateBBox(previousPosition);
        }

        public void UpdateBBox(Vector2 previousPosition)
        {
            if (Entity.IsDestroyed)
                return;

            Level.Partition.Update(this, previousPosition);
        }
    }
}