using Engine.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Collision
{
    public abstract class Collider
    {
        public Collider(Entity entity)
        {
            Entity = entity;
        }

        public event CollidedWithCallback CollidedWith;

        internal List<Point> PartitionCellPoints
        {
            get;
        } = new();

        public Entity Entity
        {
            get;
        }

        public int LayerMask
        {
            get;
            set;
        }

        public abstract RectangleF Bounds
        {
            get;
        }

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

        internal void NotifyCollidedWith(Collider body, Collision collision)
        {
            CollidedWith?.Invoke(body, collision);
        }

        public abstract bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);
        public abstract bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);

        public virtual void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
        }

        public virtual void Move(Vector2 direction)
        {
            Entity.Position += direction;
            UpdateBBox();
        }

        public virtual void MoveToPosition(Vector2 position)
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

        public virtual void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionCallback response)
        {
            var visited = new HashSet<Collider>();

            Iterate(ref velocity, layerMask, response, visited);
        }

        private void Iterate(ref Vector2 velocity, int layerMask, CollisionCallback response, HashSet<Collider> visited)
        {
            if (velocity == Vector2.Zero || Entity.IsDestroyed) 
            {
                return;
            }

            if (!TryGetNearestCollision(velocity, layerMask, out var collision))
            {
                Move(velocity);
                return;
            }

            MoveToPosition(collision.Position + 0.0001f * collision.Normal);

            if (!visited.Contains(collision.Collider))
            {
                visited.Add(collision.Collider);
                velocity = response.Invoke(collision);
                collision.Collider.NotifyCollidedWith(this, collision);
                Iterate(ref velocity, layerMask, response, visited);
            }
        }

        private bool TryGetNearestCollision(Vector2 velocity, int layerMask, out Collision collision)
        {
            collision = Collision.Empty;

            var path = new IntersectionPath(Entity.Position, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                if (collider == this) 
                {
                    continue;
                }

                if (!((collider.LayerMask | layerMask) == layerMask && broadphaseRectangle.Intersects(collider.Bounds))) 
                {
                    continue;
                }

                if (!(Intersect(collider, path, out var time, out var contact, out var normal) && time < collision.Time)) 
                {
                    continue;
                }

                collision = new Collision(collider, velocity, normal, time, contact);
            }

            return !collision.IsEmpty;
        }
    }
}
