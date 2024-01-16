using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine
{
    public sealed class World
    {
        public const int PARTITION_CELL_SIZE = 64;

        private readonly Partition partition = new(PARTITION_CELL_SIZE);
        private readonly List<Entity> entities = new();
        private readonly Dictionary<Type, List<Entity>> entitiesByType = new();
        private readonly List<Entity> entitiesToRemove = new();
        private bool sort;

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.World = this;
                AddEntity(entity);
                entity.Spawn();
            }

            foreach (var entity in entities)
            {
                entity.Start();
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
                entity.Start();
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

        public T First<T>() where T : Entity
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities)) 
            {
                return entities[0] as T;
            }
            return default;
        }

        public T FirstOrDefault<T>(Func<T, bool> predicate)
        {
            if (entitiesByType.TryGetValue(typeof(T), out var entities))
            {
                foreach (var entity in entities) 
                {
                    if (entity is T t && predicate(t))
                    {
                        return t;
                    }
                }
            }
            return default;
        }

        public void Sort()
        {
            sort = true;
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

        public IEnumerable<Entity> GetOverlappingEntities(RectangleF rectangle)
        {
            var result = new List<Entity>();

            foreach (var collider in GetNearColliders(rectangle))
            {
                if (collider.Overlaps(rectangle))
                {
                    result.Add(collider.Entity);
                }
            }

            return result;
        }

        public IEnumerable<Entity> GetOverlappingEntities(CircleF circle)
        {
            var result = new List<Entity>();

            foreach (var collider in GetNearColliders(circle))
            {
                if (collider.Overlaps(circle))
                {
                    result.Add(collider.Entity);
                }
            }

            return result;
        }

        public IEnumerable<Entity> GetOverlappingEntities(CircleF circle, int layerMask)
        {
            var result = new List<Entity>();

            foreach (var collider in GetNearColliders(circle))
            {
                if (collider.CheckMask(layerMask) && collider.Overlaps(circle))
                {
                    result.Add(collider.Entity);
                }
            }

            return result;
        }

        public IEnumerable<Collider> GetNearColliders(RectangleF bounds)
        {
            return partition.Query(bounds);
        }

        public IEnumerable<Collider> GetNearColliders(CircleF circle)
        {
            return partition.Query(circle.GetBounds());
        }

        public void Clear()
        {
            foreach (var entity in entities.ToArray())
            {
                entity.End();
            }

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

        public void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].DebugDraw(renderer, gameTime);
            }

            // Don't always need to see the partition.
            // partition.DebugDraw(renderer);
        }

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].End();
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
