using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Core.Collision;
using Core.Graphics;

namespace Core
{
    public class Level
    {
        private readonly ContentManager _content; 

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToDestroy = new();
        private readonly List<Component> _components = new();
        private readonly List<Component> _componentsToDraw = new();
        private readonly List<Component> _componentsToRemove = new();

        private Action<Entity, Component> _onComponentAdded;
        private Action<Entity, List<Component>> _onComponentsAdded;

        public Level(int cellSize, int width, int height)
        {
            _content = new ContentManager(App.Current.Services, App.ContentRoot);

            Width = width;
            Height = height;
            Camera = new Camera(this); 
            Partition = new SpatialPartition(cellSize, width, height);
        }

        public Camera Camera { get; }
        public SpatialPartition Partition { get; }

        public int Width { get; }
        public int Height { get; }

        public void Spawn(Entity entity, Vector2 position)
        {
            entity.Position = position;

            Spawn(entity);
        }

        public void Spawn(Entity entity)
        {
            entity.Level = this;

            _entities.Add(entity);

            AddComponents(entity, entity.Components);
        }

        public void Destroy(Entity entity)
        {
            if (!entity.IsDestroyed)
            {
                entity.IsDestroyed = true;

                _entitiesToDestroy.Add(entity);
            }
        }

        internal void AddComponent(Entity entity, Component component) 
        {
            component.Entity = entity;

            _components.Add(component);
            _onComponentAdded?.Invoke(entity, component);
        }

        internal void AddComponents(Entity entity, List<Component> components)
        {
            for (int i = 0; i < components.Count; i++) 
            {
                components[i].Entity = entity;
            }

            _components.AddRange(components);
            _onComponentsAdded?.Invoke(entity, components);
        }

        internal void RemoveComponent(Component component) 
        {
            _componentsToRemove.Add(component);
        }

        public virtual void Start() 
        {
            try
            {
                var copy = new List<Component>(_components);

                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(_content);
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(_content);
                    }
                };

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnLoad(_content);
                }

                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(_content);
                    c.OnSpawn();
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(_content);
                    }

                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnSpawn();
                    }
                };

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnSpawn();
                }

                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(_content);
                    c.OnSpawn();
                    c.OnStart();

                    _componentsToDraw.Add(c);
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(_content);
                    }

                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnSpawn();
                    }

                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnStart();
                    }

                    for (int i = 0; i < c.Count; i++)
                    {
                        if (c[i].IsDrawEnabled) 
                        {
                            _componentsToDraw.Add(c[i]);
                        }
                    }
                };

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnStart();
                }

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnStart();
                }

                for (int i = 0; i < copy.Count; i++)
                {
                    if (copy[i].IsDrawEnabled)
                    {
                        _componentsToDraw.Add(copy[i]);
                    }
                }
            }
            catch (StackOverflowException) 
            {
                throw new InvalidOperationException("A component either spawns itself when it's initialized or there is a circular spawn thing going on..");
            }            
        }

        public virtual void Tick(GameTime gameTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnTick(gameTime);
            }

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnPostTick(gameTime);
            }

            PostTickDestroyEntities();
            PostTickRemoveComponents();
        }

        private void PostTickDestroyEntities()
        {
            for (int i = 0; i < _entitiesToDestroy.Count; i++)
            {
                var destroyedEntity = _entitiesToDestroy[i];
                var destroyedComponents = destroyedEntity.Components;

                for (int j = 0; j < destroyedComponents.Count; j++)
                {
                    destroyedComponents[j].OnDestroy();
                }

                for (int j = 0; j < destroyedComponents.Count; j++)
                {
                    _components.Remove(destroyedComponents[j]);
                    _componentsToDraw.Remove(destroyedComponents[j]);
                    _componentsToRemove.Remove(destroyedComponents[j]);
                }

                _entities.Remove(destroyedEntity);
            }

            _entitiesToDestroy.Clear();
        }

        private void PostTickRemoveComponents()
        {
            for (int i = 0; i < _componentsToRemove.Count; i++)
            {
                _componentsToRemove[i].Entity = null;
            }

            _componentsToRemove.Clear();
        }

        public virtual void Draw(bool debug)
        {
            _componentsToDraw.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));

            for (int i = 0; i < _componentsToDraw.Count; i++)
            {
                _componentsToDraw[i].OnDraw();
            }

            if (debug)
            {
                Partition.DebugDraw();

                for (int i = 0; i < _componentsToDraw.Count; i++)
                {
                    _componentsToDraw[i].OnDebugDraw();
                }
            }
        }

        public virtual void End() 
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnDestroy();
                _components[i].OnEnd();
            }

            _content.Unload();
        }
    }
}
