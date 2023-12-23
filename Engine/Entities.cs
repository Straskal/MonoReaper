using Engine.Extensions;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    internal sealed class Entities
    {
        private readonly Level _level;

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToRemove = new();

        private readonly List<Component> _components = new();
        private readonly List<Component> _componentsToRemove = new();

        private delegate void EntityAddedHandler(Entity entity);
        private delegate void ComponentAddedHandler(params Component[] components);

        private EntityAddedHandler _entityAddedHandler;
        private ComponentAddedHandler _componentsAddedHandler;
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
                AddComponents(entity, entity.Components);
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

        public void AddComponent(Entity entity, Component component)
        {
            component.Entity = entity;
            _components.Add(component);
            _componentsAddedHandler?.Invoke(component);
            _shouldSortComponents = true;
        }

        public void AddComponents(Entity entity, List<Component> components)
        {
            components.ForEach(component => component.Entity = entity);
            _components.AddRange(components);
            _componentsAddedHandler?.Invoke(components.ToArray());
            _shouldSortComponents = true;
        }

        public void RemoveComponent(Component component)
        {
            _componentsToRemove.Add(component);
        }

        public IEnumerator Start()
        {
            yield return _entities.Load(_level.Content);
            _entityAddedHandler = OnEntityAdded_PostLoad;
            _entities.Spawn();
            _entityAddedHandler = OnEntityAdded_PostSpawn;
            _components.Spawn();
            _componentsAddedHandler = OnComponentsAdded_PostSpawn;
            _entities.Start();
            _entityAddedHandler = OnEntityAdded_PostStart;
            _components.Start();
            _componentsAddedHandler = OnComponentsAdded_PostStart;
        }

        public void Stop()
        {
            _entities.End();
            _components.End();
        }

        public void Update(GameTime gameTime)
        {
            _entities.Update(gameTime);
            _components.Update(gameTime);
            _entities.PostUpdate(gameTime);
            _components.PostUpdate(gameTime);
            ProcessDestroyedEntities();
            ProcessRemovedComponents();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            SortComponentsIfNeeded();

            _components.Draw(renderer, gameTime);
        }

        private void OnEntityAdded_PostLoad(Entity entity)
        {
            entity.OnLoad(_level.Content);
        }

        private void OnEntityAdded_PostSpawn(Entity entity)
        {
            entity.OnLoad(_level.Content);
            entity.OnSpawn();
        }

        private void OnEntityAdded_PostStart(Entity entity)
        {
            entity.OnLoad(_level.Content);
            entity.OnSpawn();
            entity.OnStart();
        }

        private void OnComponentsAdded_PostSpawn(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].OnSpawn();
            }
        }

        private void OnComponentsAdded_PostStart(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].OnSpawn();
            }

            for (int i = 0; i < components.Length; i++)
            {
                components[i].OnStart();
            }
        }

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < _entitiesToRemove.Count; i++)
            {
                var entity = _entitiesToRemove[i];
                var components = entity.Components.Take(entity.Components.Count);

                entity.OnDestroy();

                foreach (var component in components)
                {
                    component.OnDestroy();
                }

                entity.OnEnd();

                foreach (var component in components)
                {
                    component.OnEnd();
                }

                foreach (var component in entity.Components)
                {
                    component.Entity = null;

                    _components.Remove(component);
                    _componentsToRemove.Remove(component);
                }

                _entities.Remove(entity);
            }

            _entitiesToRemove.Clear();
        }

        private void ProcessRemovedComponents()
        {
            for (int i = 0; i < _componentsToRemove.Count; i++)
            {
                _componentsToRemove[i].OnEnd();
                _componentsToRemove[i].Entity = null;
                _components.Remove(_componentsToRemove[i]);
            }

            _componentsToRemove.Clear();
        }

        private void SortComponentsIfNeeded()
        {
            if (_shouldSortComponents)
            {
                _components.Sort(SortComponentsByZOrder);
                _shouldSortComponents = false;
            }
        }

        private static int SortComponentsByZOrder(Component a, Component b)
        {
            return Comparer<int>.Default.Compare(a.ZOrder, b.ZOrder);
        }
    }
}
