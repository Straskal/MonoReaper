using Engine.Extensions;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    internal sealed class Entities
    {
        private readonly Level _level;

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToRemove = new();

        private delegate void EntityAddedHandler(Entity entity);
        private EntityAddedHandler _entityAddedHandler;
        private bool _shouldSortComponents;

        public Entities(Level level)
        {
            _level = level;
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            if (entity.Level == null)
            {
                entity.Level = _level;
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
            yield return _entities.Load(_level.Content);
            _entityAddedHandler = OnEntityAdded_PostLoad;
            _entities.Spawn();
            _entityAddedHandler = OnEntityAdded_PostSpawn;
            _entities.Start();
            _entityAddedHandler = OnEntityAdded_PostStart;
        }

        public void Stop()
        {
            _entities.End();
        }

        public void Update(GameTime gameTime)
        {
            _entities.Update(gameTime);
            _entities.PostUpdate(gameTime);

            ProcessDestroyedEntities();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            SortEntititesIfNeeded();

            _entities.Draw(renderer, gameTime);
        }

        public void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            _entities.DebugDraw(renderer, gameTime);
        }

        private void OnEntityAdded_PostLoad(Entity entity)
        {
            entity.Load(_level.Content);
        }

        private void OnEntityAdded_PostSpawn(Entity entity)
        {
            entity.Load(_level.Content);
            entity.Spawn();
        }

        private void OnEntityAdded_PostStart(Entity entity)
        {
            entity.Load(_level.Content);
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
