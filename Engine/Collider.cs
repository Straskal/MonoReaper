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
        public int Layer { get; set; }
        public bool IsMoving { get; private set; }
        public CollisionResolver Resolver { get; set; } = CollisionResolvers.Slide;
        public abstract RectangleF Bounds { get; }

        public bool Intersect(Collider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            switch (collider)
            {
                case BoxCollider box:
                    return Intersect(box, path, out time, out contact, out normal);
                case CircleCollider circle:
                    return Intersect(circle, path, out time, out contact, out normal);
                default:
                    throw new ArgumentException($"Unknown collider type: {collider.GetType()}", nameof(collider));
            }
        }

        public abstract bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);
        public abstract bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);
        public abstract void DebugDraw(Renderer renderer, GameTime gameTime);

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
            contacts.Clear();
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

            var visited = new HashSet<Collider>();

            // Only iterate twice. This could potentially interfere with ignored contacts.
            if (Iterate(ref velocity, layerMask, ignoreMask, visited))
            {
                Iterate(ref velocity, layerMask, ignoreMask, visited);
            }

            IsMoving = false;
        }

        private bool Iterate(ref Vector2 velocity, int layerMask, int ignoreMask, HashSet<Collider> visited)
        {
            if (velocity == Vector2.Zero || Entity.IsDestroyed) 
            {
                return false;
            }

            var other = GetFirstCollision(velocity, layerMask, ignoreMask, visited, out var collision);

            if (other == null) 
            {
                // No collision. Just straight up move the thang and stop iterating.
                Move(velocity);
                return false;
            }

            SetPosition(collision.Contact);

            // Notify the entities about the collision.
            NotifyCollision(other, collision);
            other.NotifyCollision(this, collision);

            velocity = GetResolver(other.Layer).Invoke(collision);
            visited.Add(other);

            // Keep iterating
            return true;
        }

        private Collider GetFirstCollision(Vector2 velocity, int layerMask, int ignoreMask, HashSet<Collider> visited, out Collision collision)
        {
            collision = Collision.Empty;

            var path = new IntersectionPath(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);

            Collider other = null;

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                if (collider == this || visited.Contains(collider))
                {
                    continue;
                }

                if (!(collider.CheckMask(layerMask) && collider.CheckMask(~ignoreMask)))
                {
                    continue;
                }

                if (!(broadphaseRectangle.Intersects(collider.Bounds) && Intersect(collider, path, out var time, out var contact, out var normal) && time < collision.Time))
                {
                    continue;
                }

                other = collider;
                collision = new Collision(velocity, normal, time, contact);
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
