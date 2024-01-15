﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public abstract class Collider
    {
        private const int MAX_COLLISION_ITERATIONS = 2;

        private readonly HashSet<Collider> contacts = new();

        // Cache partition cells in the collider to avoid frequent lookups.
        internal readonly List<Point> cells = new();

        public Collider(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
        public int Layer { get; set; }
        public bool IsMoving { get; private set; }
        public IntersectionFilter Filter { get; set; }
        public abstract RectangleF Bounds { get; }

        public abstract bool Overlaps(CircleF circle);
        public abstract bool Overlaps(RectangleF rectangle);
        public abstract bool Overlaps(Collider collider);
        public abstract bool IsOverlapped(BoxCollider collider);
        public abstract bool IsOverlapped(CircleCollider collider);
        public abstract bool Intersects(Collider collider, Segment segment, out Intersection intersection);
        public abstract bool IsIntersected(BoxCollider collider, Segment segment, out Intersection intersection);
        public abstract bool IsIntersected(CircleCollider collider, Segment segment, out Intersection intersection);
        public abstract void Draw(Renderer renderer, GameTime gameTime);

        internal void NotifyCollision(Collider other, Collision collision)
        {
            if (!contacts.Contains(other))
            {
                Entity.OnCollision(other.Entity, collision);
                contacts.Add(other);
            }
        }

        internal void ClearContacts()
        {
            contacts.Clear();
        }

        public bool CheckMask(int mask)
        {
            return (mask & Layer) != 0;
        }

        public virtual void Move(Vector2 direction)
        {
            Entity.Position += direction;
            UpdateBounds();
        }

        public virtual void SetPosition(Vector2 position)
        {
            Entity.Position = position;
            UpdateBounds();
        }

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

        public List<Collider> GetOverlaps(int layerMask)
        {
            var result = new List<Collider>();
            GetOverlaps(result, layerMask);
            return result;

        }

        public void GetOverlaps(List<Collider> colliders, int layerMask)
        {
            foreach (var collider in Entity.World.GetNearColliders(Bounds))
            {
                if (collider != this && collider.CheckMask(layerMask) && Overlaps(collider))
                {
                    colliders.Add(this);
                }
            }
        }

        public Collider Cast(Vector2 velocity, int layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            var path = new Segment(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);
            Collider other = null;

            foreach (var collider in Entity.World.GetNearColliders(broadphaseRectangle))
            {
                if (collider == this)
                {
                    continue;
                }

                if (!collider.CheckMask(layerMask))
                {
                    continue;
                }

                if (!broadphaseRectangle.Overlaps(collider.Bounds))
                {
                    continue;
                }

                if (!(Intersects(collider, path, out var intersection) && intersection.Time < collision.Intersection.Time))
                {
                    continue;
                }

                if (Filter != null && Filter(collider.Entity, intersection))
                {
                    continue;
                }

                other = collider;
                collision = new Collision(velocity, intersection);
            }

            return other;
        }
    }
}
