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
            if (collider is Box box)
            {
                return Intersect(box, path, out time, out contact, out normal);
            }
            else if (collider is CircleCollider circleCollider) 
            {
                return Intersect(circleCollider, path, out time, out contact, out normal);
            }

            throw new ArgumentException();
        }

        public abstract bool Intersect(Box collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);
        public abstract bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal);

        internal void NotifyCollidedWith(Collider body, Collision collision)
        {
            CollidedWith?.Invoke(body, collision);
        }

        public virtual void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
        }
    }
}
