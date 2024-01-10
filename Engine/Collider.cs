using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public abstract class Collider
    {
        private const int MAX_ITERATIONS = 2;

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

            if (velocity.LengthSquared() > float.Epsilon)
            {
                var iterations = MAX_ITERATIONS;

                while (iterations-- > 0)
                {
                    var other = Cast(velocity, layerMask, ignoreMask, out var collision);

                    if (other == null)
                    {
                        Move(velocity);
                        break;
                    }
                    
                    if (collision.Time > ContactOffset)
                    {
                        Move(collision.Direction * (collision.Time - ContactOffset));
                    }

                    velocity = CollisionResolvers.Slide(collision);
                    NotifyCollision(other, collision);
                    other.NotifyCollision(this, collision);
                };
            }  

            IsMoving = false;
        }

        private Collider Cast(Vector2 velocity, int layerMask, int ignoreMask, out Collision collision)
        {
            collision = Collision.Empty;
            var path = new Segment(Bounds.Center, velocity);
            var broadphaseRectangle = Bounds.Union(velocity);
            Collider other = null;

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                // Don't intersect self
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
                collision = new Collision(velocity, intersection); ;
            }

            return other;
        }
    }
}
