using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    internal sealed class Entities
    {
        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToRemove = new();

        private delegate void EntityAddedHandler(Entity entity);
        private EntityAddedHandler _entityAddedHandler;
        private bool _shouldSortComponents;

        public Entities(Level level)
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

                _entities.Add(entity);
                _entityAddedHandler?.Invoke(entity);
            }
        }

        public void Destroy(Entity entity)
        {
            if (!entity.IsDestroyed)
            {
                entity.IsDestroyed = true;
                _entitiesToRemove.Add(entity);
            }
        }

        public IEnumerator Start()
        {
            var currentAssetCount = Level.Content.LoadedAssetCount;

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Load(Level.Content);

                if (currentAssetCount != Level.Content.LoadedAssetCount)
                {
                    currentAssetCount = Level.Content.LoadedAssetCount;
                    yield return null;
                }
            }

            _entityAddedHandler = OnEntityAdded_PostLoad;

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Spawn();
            }

            _entityAddedHandler = OnEntityAdded_PostSpawn;

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Start();
            }

            _entityAddedHandler = OnEntityAdded_PostStart;
        }

        public void Stop()
        {
            foreach (var entity in _entities.ToArray())
            {
                entity.End();
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update(gameTime);
            }

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].PostUpdate(gameTime);
            }

            ProcessDestroyedEntities();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            SortEntititesIfNeeded();

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Draw(renderer, gameTime);
            }
        }

        public void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].DebugDraw(renderer, gameTime);
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
            for (int i = 0; i < _entitiesToRemove.Count; i++)
            {
                _entitiesToRemove[i].Destroy();
                _entitiesToRemove[i].End();
                _entitiesToRemove[i].Level = null;
                _entities.Remove(_entitiesToRemove[i]);
            }

            _entitiesToRemove.Clear();
        }

        private void SortEntititesIfNeeded()
        {
            if (_shouldSortComponents)
            {
                _entities.Sort(SortComponentsByZOrder);
                _shouldSortComponents = false;
            }
        }

        private static int SortComponentsByZOrder(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.GraphicsComponent.DrawOrder, b.GraphicsComponent.DrawOrder);
        }
    }
}
