using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public sealed class World
    {
        public const int PARTITION_CELL_SIZE = 64;

        private readonly Partition partition = new(PARTITION_CELL_SIZE);
        private readonly List<Entity> entities = new();
        private readonly List<Entity> entitiesToRemove = new();
        private bool sort;

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.World = this;
                this.entities.Add(entity);
                entity.Spawn();
            }

            foreach (var entity in entities)
            {
                entity.Start();
            }
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
                entities.Add(entity);
                entity.Spawn();
                entity.Start();
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

        public T Find<T>()
        {
            foreach (var entity in entities)
            {
                if (entity is T t)
                {
                    return t;
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

        public IEnumerable<Entity> GetOverlappingEntities(CircleF circle, int layerMask)
        {
            var result = new List<Entity>();

            foreach (var collider in GetCollidersWithinBounds(circle))
            {
                if (collider.CheckMask(layerMask) && collider.Overlaps(circle))
                {
                    result.Add(collider.Entity);
                }
            }

            return result;
        }

        public IEnumerable<Collider> GetCollidersWithinBounds(RectangleF bounds)
        {
            return partition.Query(bounds);
        }

        public IEnumerable<Collider> GetCollidersWithinBounds(CircleF circle)
        {
            return partition.Query(new RectangleF(
                circle.Center.X - circle.Radius,
                circle.Center.Y - circle.Radius,
                circle.Radius * 2f,
                circle.Radius * 2f));
        }

        public void Clear()
        {
            foreach (var entity in entities.ToArray())
            {
                entity.End();

                if (entity.Collider != null)
                {
                    partition.Remove(entity.Collider);
                }
            }

            entities.Clear();
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

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Collider?.ClearContacts();
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
                entities.Remove(entitiesToRemove[i]);
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

        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }
    }
}
