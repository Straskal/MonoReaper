using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    internal sealed class EntityManager
    {
        private readonly List<Entity> entities = new();
        private readonly List<Entity> entitiesToRemove = new();
        private delegate void EntityAddedHandler(Entity entity);
        private EntityAddedHandler entityAddedHandler;
        private bool shouldSortComponents;

        public EntityManager(Level level)
        {
            Level = level;
        }

        public Level Level { get; }

        public void Spawn(Entity entity, Vector2 position)
        {
            if (entity.Level == null)
            {
                entity.Level = Level;
                entity.Position = position;
                entities.Add(entity);
                entityAddedHandler?.Invoke(entity);
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

        public IEnumerator Start()
        {
            var currentAssetCount = Level.Content.LoadedAssetCount;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Load(Level.Content);

                if (currentAssetCount != Level.Content.LoadedAssetCount)
                {
                    currentAssetCount = Level.Content.LoadedAssetCount;
                    yield return null;
                }
            }

            entityAddedHandler = OnEntityAdded_PostLoad;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Spawn();
            }

            entityAddedHandler = OnEntityAdded_PostSpawn;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Start();
            }

            entityAddedHandler = OnEntityAdded_PostStart;
        }

        public void Stop()
        {
            foreach (var entity in entities.ToArray())
            {
                entity.End();
            }
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
        }

        private void OnEntityAdded_PostLoad(Entity entity)
        {
            entity.Load(Level.Content);
        }

        private void OnEntityAdded_PostSpawn(Entity entity)
        {
            entity.Load(Level.Content);
            entity.Spawn();
        }

        private void OnEntityAdded_PostStart(Entity entity)
        {
            entity.Load(Level.Content);
            entity.Spawn();
            entity.Start();
        }

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].End();
                entitiesToRemove[i].Level = null;
                entities.Remove(entitiesToRemove[i]);
            }

            entitiesToRemove.Clear();
        }

        private void SortEntititesIfNeeded()
        {
            if (shouldSortComponents)
            {
                entities.Sort(SortComponentsByZOrder);
                shouldSortComponents = false;
            }
        }

        private static int SortComponentsByZOrder(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.GraphicsComponent.DrawOrder, b.GraphicsComponent.DrawOrder);
        }
    }
}
