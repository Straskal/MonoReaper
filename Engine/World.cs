using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Engine
{
    public sealed class World : IEnumerable<Entity>
    {
        private readonly List<Entity> entities;
        private readonly List<Entity> entitiesToRemove;
        private readonly List<Collider> colliders;
        private bool sort;

        public World()
        {
            entities = new List<Entity>();
            entitiesToRemove = new List<Entity>();
            colliders = new List<Collider>();
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.World = this;
                entity.IsAlive = true;
                this.entities.Add(entity);
                entity.Spawn();
            }
            sort = true;
        }

        public void Spawn(Entity entity)
        {
            Spawn(entity, entity.Position);
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            if (entity.World == null)
            {
                entity.World = this;
                entity.IsAlive = true;
                entity.Position = position;
                entities.Add(entity);
                entity.Spawn();
                sort = true;
            }
        }

        public void Destroy(Entity entity)
        {
            if (entity.IsAlive)
            {
                entity.IsAlive = false;
                entitiesToRemove.Add(entity);
            }
        }

        public void Clear()
        {
            entities.Clear();
            entitiesToRemove.Clear();
            colliders.Clear();
        }

        public T Find<T>() where T : Entity
        {
            foreach (var entity in entities)
            {
                if (entity is T t)
                {
                    return t;
                }
            }
            return null;
        }

        public T FindWithTag<T>(string tag) where T : Entity
        {
            foreach (var entity in entities)
            {
                if (entity is T t && entity.Tags.Contains(tag))
                {
                    return t;
                }
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(gameTime);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PostUpdate(gameTime);
            }

            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].Collider?.Disable();
                entitiesToRemove[i].World = null;
                entities.Remove(entitiesToRemove[i]);
            }

            entitiesToRemove.Clear();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            if (sort)
            {
                entities.Sort(SortEntities);
                sort = false;
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Draw(renderer, gameTime);
            }
        }

        public void DebugDraw(Renderer renderer)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].DebugDraw(renderer);
            }

            foreach (var collider in colliders)
            {
                collider.Draw(renderer);
            }
        }

        #region Collision

        public void EnableCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        public void DisableCollider(Collider collider)
        {
            colliders.Remove(collider);
        }

        public List<Collider> OverlapColliders(Vector2 point)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Vector2 point, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Vector2 point, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(CircleF circle, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(CircleF circle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(RectangleF rectangle, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(RectangleF rectangle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Collider collider, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var other in colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Overlaps(collider))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(CircleF circle, uint layerMask, Collider ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.OverlapCircle(circle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(RectangleF rectangle, uint layerMask, Collider ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.OverlapRectangle(rectangle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(Collider collider, uint layerMask) where T : Entity
        {
            var result = new List<T>();

            foreach (var other in colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Entity is T t && other.Overlaps(collider))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public Collider Cast(Collider collider, Vector2 direction, uint layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            var path = new Segment(collider.Bounds.Center, direction);
            Collider collidedWith = null;

            foreach (var other in OverlapColliders(collider.Bounds.Union(direction), layerMask, collider))
            {
                if (collider.Intersects(other, path, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    collidedWith = other;
                    collision = new Collision(direction, intersection);
                }
            }

            return collidedWith;
        }

        // TODO: This needs some love
        public Collider LineCast(Vector2 position, Vector2 direction, uint layerMask, Collider ignore)
        {
            var collision = Collision.Empty;
            var segment = new Segment(position, direction);
            var broadphaseRectangle = new RectangleF(
                MathF.Min(position.X, position.X + direction.X),
                MathF.Min(position.Y, position.Y + direction.Y),
                MathF.Abs(direction.X),
                MathF.Abs(direction.Y));

            Collider collidedWith = default;

            // TODO: Overlap segment
            foreach (var collider in OverlapColliders(broadphaseRectangle, layerMask, ignore))
            {
                if (collider.IntersectSegment(segment, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    collidedWith = collider;
                    collision = new Collision(direction, intersection);
                }
            }

            return collidedWith;
        }

        #endregion Collision

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(Collider collider, uint layerMask)
        {
            return collider.CheckMask(layerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(Collider collider, uint layerMask, Collider ignore)
        {
            return collider.CheckMask(layerMask) && collider != ignore;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }
    }
}
