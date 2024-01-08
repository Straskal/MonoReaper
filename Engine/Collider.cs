using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine
{
    public abstract class Collider
    {
        private readonly HashSet<Collider> notifiedContacts = new();
        private readonly Dictionary<int, CollisionResponseType> collisionResponses = new();

        public Collider(Entity entity)
        {
            Entity = entity;
        }

        internal List<Point> Cells { get; } = new();
        public Entity Entity { get; }
        public int Layer { get; set; }
        public bool IsMoving { get; private set; }
        public CollisionResponseType DefaultResponse { get; set; } = CollisionResponseType.Slide;
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

        public void AddResponse(int layer, CollisionResponseType responseType) 
        {
            collisionResponses.Add(layer, responseType);
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

        public bool CheckMask(int mask) 
        {
            return (mask & Layer) != 0;
        }

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
            notifiedContacts.Add(collider);
        }

        internal bool HasContact(Collider collider)
        {
            return notifiedContacts.Contains(collider);
        }

        internal void ClearContacts()
        {
            notifiedContacts.Clear();
        }

        public virtual void MoveAndCollide(ref Vector2 velocity)
        {
            MoveAndCollide(ref velocity, int.MaxValue, 0);
        }

        public virtual void MoveAndCollide(ref Vector2 velocity, int layerMask)
        {
            MoveAndCollide(ref velocity, layerMask, 0);
        }

        public virtual void MoveAndCollide(ref Vector2 velocity, int layerMask, int ignoreMask)
        {
            IsMoving = true;

            var visited = new HashSet<Collider>();

            if (RunMovementIteration(ref velocity, layerMask, ignoreMask, visited))
            {
                RunMovementIteration(ref velocity, layerMask, ignoreMask, visited);
            }

            IsMoving = false;
        }

        private bool RunMovementIteration(ref Vector2 velocity, int layerMask, int ignoreMask, HashSet<Collider> visited)
        {
            var runAnotherIteration = false;

            if (velocity != Vector2.Zero && !Entity.IsDestroyed && TryGetFirstCollision(velocity, layerMask, ignoreMask, visited, out var hit, out var collision))
            {
                SetPosition(collision.Position);
                NotifyCollision(hit, collision);
                hit.NotifyCollision(this, collision);

                if (!collisionResponses.TryGetValue(hit.Layer, out var response)) 
                {
                    response = DefaultResponse;
                }

                switch (response) 
                {
                    case CollisionResponseType.Ignore:
                        velocity = collision.Ignore();
                        break;
                    case CollisionResponseType.Bounce:
                        velocity = collision.Bounce();
                        break;
                    default:
                        velocity = collision.Slide();
                        break;
                }

                visited.Add(hit);
                runAnotherIteration = true;
            }
            else
            {
                Move(velocity);
            }

            return runAnotherIteration;
        }

        private bool TryGetFirstCollision(Vector2 velocity, int layerMask, int ignoreMask, HashSet<Collider> visited, out Collider hit, out Collision collision)
        {
            hit = null;
            collision = Collision.Empty;

            var path = new IntersectionPath(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);

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

                hit = collider;
                collision = new Collision(velocity, normal, time, contact);
            }

            return !collision.IsEmpty;
        }
    }
}
