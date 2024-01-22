using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public sealed class World : IEnumerable<Entity>
    {
        private readonly List<Entity> entities;
        private readonly List<Entity> entitiesToRemove;
        private readonly Dictionary<Type, List<Entity>> entitiesByType;
        private readonly List<Collider> colliders;
        private bool sort;

        public World()
        {
            entities = new List<Entity>();
            entitiesToRemove = new List<Entity>();
            entitiesByType = new Dictionary<Type, List<Entity>>();
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
                AddEntity(entity);
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
                AddEntity(entity);
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
            entitiesByType.Clear();
            colliders.Clear();
        }

        public T Find<T>() where T : Entity
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities))
            {
                return (T)entities[0];
            }
            return null;
        }

        public T FindWithTag<T>(string tag) where T : Entity
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities))
            {
                foreach (var entity in entities)
                {
                    if (entity.Tags.Contains(tag))
                    {
                        return (T)entity;
                    }
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

            ProcessDestroyedEntities();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            SortEntititesIfNeeded();

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

        public List<Collider> OverlapPoint(Vector2 point)
        {
            return OverlapPoint(point, uint.MaxValue, null);
        }

        public List<Collider> OverlapPoint(Vector2 point, uint layerMask)
        {
            return OverlapPoint(point, layerMask, null);
        }

        public List<Collider> OverlapPoint(Vector2 point, uint layerMask, Collider ignore)
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

        public List<Collider> OverlapCircle(CircleF circle)
        {
            return OverlapCircle(circle, uint.MaxValue, null);
        }

        public List<Collider> OverlapCircle(CircleF circle, uint layerMask)
        {
            return OverlapCircle(circle, layerMask, null);
        }

        public List<Collider> OverlapCircle(CircleF circle, uint layerMask, Collider ignore)
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

        public List<T> OverlapCircle<T>(CircleF circle, uint layerMask, Collider ignore) where T : Entity
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

        public List<Collider> OverlapRectangle(RectangleF rectangle)
        {
            return OverlapRectangle(rectangle, uint.MaxValue, null);
        }

        public List<Collider> OverlapRectangle(RectangleF rectangle, uint layerMask)
        {
            return OverlapRectangle(rectangle, layerMask, null);
        }

        public List<Collider> OverlapRectangle(RectangleF rectangle, uint layerMask, Collider ignore)
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

        public List<T> OverlapRectangle<T>(RectangleF rectangle, uint layerMask, Collider ignore) where T : Entity
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

        public List<Collider> OverlapCollider(Collider collider)
        {
            return OverlapCollider(collider, uint.MaxValue);
        }

        public List<Collider> OverlapCollider(Collider collider, uint layerMask)
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

        public List<T> OverlapCollider<T>(Collider collider, uint layerMask) where T : Entity
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
            var broadphaseRectangle = collider.Bounds.Union(direction);
            Collider collidedWith = null;

            foreach (var other in OverlapRectangle(broadphaseRectangle, layerMask, collider))
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
            foreach (var collider in OverlapRectangle(broadphaseRectangle, layerMask, ignore))
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

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].Collider?.Disable();
                entitiesToRemove[i].World = null;
                RemoveEntity(entitiesToRemove[i]);
            }

            entitiesToRemove.Clear();
        }

        private void SortEntititesIfNeeded()
        {
            if (sort)
            {
                entities.Sort(SortEntities);
                sort = false;
            }
        }

        private void AddEntity(Entity entity)
        {
            var type = entity.GetType();
            entities.Add(entity);
            if (!entitiesByType.TryGetValue(type, out var list))
            {
                entitiesByType[type] = list = new List<Entity>();
            }
            list.Add(entity);
        }

        private void RemoveEntity(Entity entity)
        {
            var type = entity.GetType();
            entities.Remove(entity);
            if (!entitiesByType.TryGetValue(type, out var list))
            {
                entitiesByType[type] = list = new List<Entity>();
            }
            list.Remove(entity);
            if (list.Count == 0)
            {
                entitiesByType.Remove(type);
            }
        }

        private static bool CanOverlapCollider(Collider collider, uint layerMask, Collider ignore)
        {
            return collider.CheckMask(layerMask) && collider != ignore;
        }

        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }
    }
}
