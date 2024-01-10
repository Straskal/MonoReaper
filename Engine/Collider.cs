using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine
{
    public abstract class Collider
    {
        private readonly HashSet<Collider> contacts = new();
        private readonly Dictionary<int, CollisionResolver> resolvers = new();

        // Cache partition cells in the collider to avoid frequent lookups.
        internal readonly List<Point> cells = new();

        public Collider(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
        public CollisionResolver Resolver { get; set; } = CollisionResolvers.Slide;
        public int Layer { get; set; }
        public bool IsMoving { get; private set; }
        public abstract RectangleF Bounds { get; }
        public float ContactOffset { get; } = 0.001f;

        public abstract bool Overlaps(Collider collider);
        public abstract bool IsOverlapped(BoxCollider collider);
        public abstract bool IsOverlapped(CircleCollider collider);
        public abstract bool Intersects(Collider collider, Segment segment, out Intersection intersection);
        public abstract bool IsIntersected(BoxCollider collider, Segment segment, out Intersection intersection);
        public abstract bool IsIntersected(CircleCollider collider, Segment segment, out Intersection intersection);
        public abstract void Draw(Renderer renderer, GameTime gameTime);

        internal void NotifyCollision(Collider other, Collision collision)
        {
            if (!HasContact(other))
            {
                Entity.Collision(other.Entity, collision);
                AddContact(other);
            }
        }

        internal void AddContact(Collider collider)
        {
            contacts.Add(collider);
        }

        internal bool HasContact(Collider collider)
        {
            return contacts.Contains(collider);
        }

        internal void ClearContacts()
        {
            contacts.RemoveWhere(c => !Overlaps(c));
        }

        public IEnumerable<Collider> GetContacts() 
        {
            return contacts;
        }

        public void AddResolver(int layer, CollisionResolver resolver) 
        {
            resolvers.Add(layer, resolver);
        }

        public bool CheckMask(int mask)
        {
            return (mask & Layer) != 0;
        }

        public virtual void Move(Vector2 direction)
        {
            Entity.Position += direction;
            UpdateBBox();
        }

        public virtual void SetPosition(Vector2 position)
        {
            Entity.Position = position;
            UpdateBBox();
        }

        public virtual void Register()
        {
            Entity.Level.Partition.Add(this);
        }

        public virtual void Unregister()
        {
            Entity.Level.Partition.Remove(this);
        }

        public virtual void UpdateBBox()
        {
            Entity.Level.Partition.Update(this);
        }

        public void Collide(Vector2 velocity)
        {
            Collide(ref velocity, int.MaxValue, 0);
        }

        public void Collide(Vector2 velocity, int layerMask)
        {
            Collide(ref velocity, layerMask, 0);
        }

        public void Collide(Vector2 velocity, int layerMask, int ignoreMask)
        {
            Collide(ref velocity, layerMask, ignoreMask);
        }

        public void Collide(ref Vector2 velocity)
        {
            Collide(ref velocity, int.MaxValue, 0);
        }

        public void Collide(ref Vector2 velocity, int layerMask)
        {
            Collide(ref velocity, layerMask, 0);
        }

        public virtual void Collide(ref Vector2 velocity, int layerMask, int ignoreMask)
        {
            IsMoving = true;

            const int MAX_ITERATIONS = 5;

            for (int i = 0; i < MAX_ITERATIONS; i++) 
            {
                if (!Iterate(ref velocity, layerMask, ignoreMask))
                {
                    break;
                }
            } 

            IsMoving = false;
        }

        private bool Iterate(ref Vector2 velocity, int layerMask, int ignoreMask)
        {
            if (velocity == Vector2.Zero || Entity.IsDestroyed) 
            {
                return false;
            }

            var other = GetFirstCollision(velocity, layerMask, ignoreMask, out var collision);

            if (other == null) 
            {
                Move(velocity);
                return false;
            }

            // Use offset time to prevent colliders from ending in a touching position.
            var offsetTime = MathF.Max(0f, collision.Time - ContactOffset);
            SetPosition(Bounds.Center + collision.Direction * offsetTime);
            velocity = GetResolver(other.Layer).Invoke(collision);

            NotifyCollision(other, collision);
            other.NotifyCollision(this, collision);

            return true;
        }

        private Collider GetFirstCollision(Vector2 velocity, int layerMask, int ignoreMask, out Collision collision)
        {
            collision = Collision.Empty;

            var path = new Segment(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);

            Collider other = null;

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                if (collider == this)
                {
                    continue;
                }

                if (!(collider.CheckMask(layerMask) && collider.CheckMask(~ignoreMask)))
                {
                    continue;
                }

                if (!(broadphaseRectangle.Overlaps(collider.Bounds) && Intersects(collider, path, out var intersection) && intersection.Time < collision.Time))
                {
                    continue;
                }

                other = collider;
                collision = new Collision(velocity, intersection);
            }

            return other;
        }

        private CollisionResolver GetResolver(int layer)
        {
            if (!resolvers.TryGetValue(layer, out var resolver))
            {
                resolver = Resolver;
            }

            return resolver;
        }
    }
}
