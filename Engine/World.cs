using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public sealed class World : IEnumerable<Entity>
    {
        private readonly Partition partition;
        private readonly List<Entity> entities;
        private readonly Dictionary<Type, List<Entity>> entitiesByType;
        private readonly List<Entity> entitiesToRemove;
        private bool sort;

        public World(int cellSize) 
        {
            partition = new Partition(cellSize);
            entities = new List<Entity>();
            entitiesByType = new Dictionary<Type, List<Entity>>();
            entitiesToRemove = new List<Entity>();
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
                AddEntity(entity);
                entity.Spawn();
            }
            Sort();
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
                entity.Position = position;
                AddEntity(entity);
                entity.Spawn();
                Sort();
            }
        }

        public void Destroy(Entity entity)
        {
            if (!entity.IsDestroyed)
            {
                entity.IsDestroyed = true;
                entitiesToRemove.Add(entity);
            }
        }

        public T Find<T>() where T : Entity
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities))
            {
                return entities[0] as T;
            }
            return default;
        }

        public T FindWithTag<T>(string tag) where T : Entity
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities))
            {
                foreach (var entity in entities) 
                {
                    if (entity.Tags.Contains(tag)) 
                    {
                        return entity as T;
                    }
                }
            }
            return default;
        }

        public void EnableCollider(Collider collider)
        {
            partition.Add(collider);
        }

        public void DisableCollider(Collider collider)
        {
            partition.Remove(collider);
        }

        public void UpdateCollider(Collider collider)
        {
            partition.Update(collider);
        }

        public void Sort()
        {
            sort = true;
        }

        public bool OverlapPointAny(Vector2 point)
        {
            return OverlapPoint(point, uint.MaxValue, null).Any();
        }

        public bool OverlapPointAny(Vector2 point, uint layerMask)
        {
            return OverlapPoint(point, layerMask, null).Any();
        }

        public bool OverlapPointAny(Vector2 point, uint layerMask, Collider ignore) 
        {
            return OverlapPoint(point, layerMask, ignore).Any();
        }

        public IEnumerable<Collider> OverlapPoint(Vector2 point)
        {
            return OverlapPoint(point, uint.MaxValue, null);
        }

        public IEnumerable<Collider> OverlapPoint(Vector2 point, uint layerMask)
        {
            return OverlapPoint(point, layerMask, null);
        }

        public IEnumerable<Collider> OverlapPoint(Vector2 point, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in partition.Query(point))
            {
                if (collider != ignore && collider.CheckMask(layerMask))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public IEnumerable<Collider> OverlapCircle(CircleF circle)
        {
            return OverlapCircle(circle, uint.MaxValue, null);
        }

        public IEnumerable<Collider> OverlapCircle(CircleF circle, uint layerMask)
        {
            return OverlapCircle(circle, layerMask, null);
        }

        public IEnumerable<Collider> OverlapCircle(CircleF circle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in partition.Query(circle))
            {
                if (collider != ignore && collider.CheckMask(layerMask))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public IEnumerable<Collider> OverlapRectangle(RectangleF rectangle)
        {
            return OverlapRectangle(rectangle, uint.MaxValue, null);
        }

        public IEnumerable<Collider> OverlapRectangle(RectangleF rectangle, uint layerMask)
        {
            return OverlapRectangle(rectangle, layerMask, null);
        }

        public IEnumerable<Collider> OverlapRectangle(RectangleF rectangle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in partition.Query(rectangle))
            {
                if (collider != ignore && collider.CheckMask(layerMask))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

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

        public void Clear()
        {
            entities.Clear();
            entitiesByType.Clear();
            partition.Clear();
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
            partition.DebugDraw(renderer);

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].DebugDraw(renderer);
            }
        }

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

        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }
    }
}
