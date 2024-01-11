using Microsoft.Xna.Framework;
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
        public float ContactOffset { get; } = 0.001f;
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
                Entity.Collision(other.Entity, collision);
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
            Entity.Level.Partition.Add(this);
        }

        public virtual void Disable()
        {
            Entity.Level.Partition.Remove(this);
        }

        public virtual void UpdateBounds()
        {
            Entity.Level.Partition.Update(this);
        }

        public void Collide(Vector2 velocity)
        {
            Collide(ref velocity, int.MaxValue);
        }

        public void Collide(Vector2 velocity, int layerMask)
        {
            Collide(ref velocity, layerMask);
        }

        public void Collide(ref Vector2 velocity)
        {
            Collide(ref velocity, int.MaxValue);
        }

        public virtual void Collide(ref Vector2 velocity, int layerMask)
        {
            IsMoving = true;

            if (velocity.LengthSquared() > float.Epsilon)
            {
                var iterations = MAX_COLLISION_ITERATIONS;

                while (iterations-- > 0)
                {
                    var other = Cast(velocity, layerMask, out Collision collision);

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

        public List<Collider> GetOverlaps(int layerMask)
        {
            var result = new List<Collider>();
            GetOverlaps(result, layerMask);
            return result;

        }

        public void GetOverlaps(List<Collider> colliders, int layerMask) 
        {
            foreach (var collider in Entity.Level.Partition.Query(Bounds))
            {
                if (collider != this && collider.CheckMask(layerMask) && Overlaps(collider)) 
                {
                    colliders.Add(this);
                }
            }
        }

        private Collider Cast(Vector2 velocity, int layerMask, out Collision collision)
        {
            var collider = Cast(velocity, layerMask, out Intersection intersection);
            collision = new Collision(velocity, intersection);
            return collider;
        }

        public Collider Cast(Vector2 direction, int layerMask, out Intersection intersection) 
        {
            intersection = Intersection.Empty;
            var time = float.PositiveInfinity;
            var path = new Segment(Bounds.Center, direction);
            var broadphaseRectangle = Bounds.Union(direction);
            Collider other = null;

            foreach (var collider in Entity.Level.Partition.Query(broadphaseRectangle))
            {
                if (!(collider != this && collider.CheckMask(layerMask) && broadphaseRectangle.Overlaps(collider.Bounds)))
                {
                    continue;
                }

                if (!(Intersects(collider, path, out var inter) && inter.Time < time))
                {
                    continue;
                }

                if (Filter != null && Filter(collider.Entity, inter))
                {
                    continue;
                }

                other = collider;
                intersection = inter;
            }

            return other;
        }
    }
}
