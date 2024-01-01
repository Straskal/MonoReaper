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

        public abstract bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);
        public abstract bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);

        internal void NotifyCollidedWith(Collider body, Collision collision)
        {
            CollidedWith?.Invoke(body, collision);
        }

        public virtual void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
        }

        public void Move(Vector2 direction)
        {
            Entity.Position += direction;
            UpdateBBox();
        }

        public void MoveToPosition(Vector2 position)
        {
            Entity.Position = position;
            UpdateBBox();
        }

        public void UpdateBBox()
        {
            Entity.Level.Partition.Update(this);
        }

        public void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionCallback response)
        {
            // This loop makes me nervous. Should probably limit the number of iterations.
            Collider last = null;

            while (!(velocity == Vector2.Zero || Entity.IsDestroyed) && RunMovementIteration(ref velocity, layerMask, response, last, out var collision)) 
            {
                last = collision.Collider;
            }
        }

        private bool RunMovementIteration(ref Vector2 velocity, int layerMask, CollisionCallback response, Collider previousCollider, out Collision collision)
        {
            if (!TryGetNearestCollision(velocity, layerMask, previousCollider, out collision))
            {
                Move(velocity);
                return false;
            }

            MoveToPosition(collision.Position);
            velocity = response.Invoke(collision);
            collision.Collider.NotifyCollidedWith(this, collision);
            return true;
        }

        private bool TryGetNearestCollision(Vector2 velocity, int layerMask, Collider previousCollider, out Collision collision)
        {
            collision = Collision.Empty;

            var path = new IntersectionPath(Entity.Position, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                if (collider == this || collider == previousCollider) 
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
