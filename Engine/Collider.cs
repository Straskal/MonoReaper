﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public abstract class Collider
    {
        // Cache partition cells in the collider to avoid frequent lookups.
        internal readonly List<Point> cells = new();

        public Collider(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
        public uint Layer { get; set; }

        public abstract RectangleF Bounds { get; }
        public abstract bool Overlaps(Collider collider);
        public abstract bool OverlapPoint(Vector2 point);
        public abstract bool OverlapCircle(CircleF circle);
        public abstract bool OverlapRectangle(RectangleF rectangle);
        public abstract bool Intersects(Collider collider, Segment segment, out Intersection intersection);
        public abstract bool IntersectSegment(Segment segment, out Intersection intersection);
        public abstract bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection);
        public abstract bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection);
        public abstract void Draw(Renderer renderer, GameTime gameTime);

        public virtual void Enable()
        {
            Entity.World.EnableCollider(this);
        }

        public virtual void Disable()
        {
            Entity.World.DisableCollider(this);
        }

        public virtual void UpdateBounds()
        {
            Entity.World.UpdateCollider(this);
        }

        public bool CheckMask(uint mask)
        {
            return (mask & Layer) != 0;
        }

        public IEnumerable<Collider> GetOverlaps(uint layerMask)
        {
            return Entity.World.OverlapRectangle(Bounds, layerMask, this);
        }

        public Collider Cast(Vector2 velocity, uint layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            var path = new Segment(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);
            Collider collidedWith = null;

            foreach (var collider in Entity.World.OverlapRectangle(broadphaseRectangle, layerMask, this))
            {
                if (Intersects(collider, path, out var intersection) && intersection.Time < collision.Intersection.Time) 
                {
                    collidedWith = collider;
                    collision = new Collision(velocity, intersection);
                }          
            }

            return collidedWith;
        }
    }
}
