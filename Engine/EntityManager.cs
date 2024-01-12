using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public sealed class EntityManager
    {
        public const int PARTITION_CELL_SIZE = 64;

        private readonly Partition partition = new(PARTITION_CELL_SIZE);
        private readonly List<Entity> entities = new();
        private readonly List<Entity> entitiesToRemove = new();
        private bool shouldSortComponents;

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities) 
            {
                entity.Others = this;
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
            if (entity.Others == null)
            {
                entity.Others = this;
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

        public IEnumerable<Collider> GetCollidersWithinBounds(RectangleF bounds) 
        {
            return partition.Query(bounds);
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

            partition.DebugDraw(renderer);
        }

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].End();
                entitiesToRemove[i].Others = null;
                entities.Remove(entitiesToRemove[i]);
            }

            entitiesToRemove.Clear();
        }

        private void SortEntititesIfNeeded()
        {
            if (shouldSortComponents)
            {
                entities.Sort(SortEntities);
                shouldSortComponents = false;
            }
        }

        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.GraphicsComponent.DrawOrder, b.GraphicsComponent.DrawOrder);
        }
    }
}
