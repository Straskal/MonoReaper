using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Core.Collision;
using Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine.Graphics;

namespace Core
{
    public class Level
    {
        // Content manager to hold level specific assets
        protected readonly ContentManager content;

        // Post processing effects
        protected readonly List<PostProcessingEffect> postProcessingEffects = new();

        // All entities in the level
        private readonly List<Entity> _entities = new();

        // Entities to destroy at the end of the current frame
        private readonly List<Entity> _entitiesToDestroy = new();

        // All components in the level
        private readonly List<Component> _components = new();

        // All components that requested to be drawn
        private readonly List<Component> _componentsToDraw = new();

        // All components to remove at the of the current frame
        private readonly List<Component> _componentsToRemove = new();

        // Component added handlers
        private Action<Entity, Component> _onComponentAdded;
        private Action<Entity, List<Component>> _onComponentsAdded;

        public Level(int cellSize, int width, int height)
        {
            content = new ContentManager(App.Current.Services, App.ContentRoot);

            Width = width;
            Height = height;
            Camera = new Camera(App.ViewportWidth, App.ViewportHeight);
            RenderTarget = new RenderTarget2D(App.GraphicsDeviceManager.GraphicsDevice, Resolution.RenderTargetResolution.width, Resolution.RenderTargetResolution.height);
            Partition = new Partition(cellSize, width, height);
        }

        public Camera Camera { get; }
        public RenderTarget2D RenderTarget { get; }
        public Partition Partition { get; }

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

        public void AddPostProcessingEffect(PostProcessingEffect effect) 
        {
            postProcessingEffects.Add(effect);
        }

        internal void AddComponent(Entity entity, Component component) 
        {
            component.Entity = entity;

            _components.Add(component);
            _onComponentAdded?.Invoke(entity, component);
        }

        internal void AddComponents(Entity entity, List<Component> components)
        {
            var copy = new List<Component>(components);

            foreach (var component in copy) 
            {
                component.Entity = entity;
            }

            _components.AddRange(copy);
            _onComponentsAdded?.Invoke(entity, copy);
        }

        internal void RemoveComponent(Component component) 
        {
            _componentsToRemove.Add(component);
        }

        public virtual void Start() 
        {
            try
            {
                var components = _components;

                // Invoke callback on component list and any new additions during this loop.
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].OnLoad(content);
                }

                // After all tracked components are handled, update the component added handlers to invoke the callback.
                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(content);
                    }
                };

                for (int i = 0; i < components.Count; i++)
                {
                    components[i].OnSpawn();
                }

                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                    c.OnSpawn();
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(content);
                    }

                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnSpawn();
                    }
                };

                for (int i = 0; i < components.Count; i++)
                {
                    components[i].OnStart();
                }

                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i].IsDrawEnabled)
                    {
                        _componentsToDraw.Add(components[i]);
                    }
                }

                _onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                    c.OnSpawn();
                    c.OnStart();

                    if (c.IsDrawEnabled) 
                    {
                        _componentsToDraw.Add(c);
                    }         
                };

                _onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(content);
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

            foreach (var effect in postProcessingEffects) 
            {
                effect.OnTick(gameTime);
            }
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
                _componentsToRemove[i].OnDestroy();
                _componentsToRemove[i].Entity = null;

                _components.Remove(_componentsToRemove[i]);
                _componentsToDraw.Remove(_componentsToRemove[i]);
            }

            _componentsToRemove.Clear();
        }

        public virtual Texture2D Draw(bool debug)
        {
            _componentsToDraw.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));

            var currentRenderTarget = RenderTarget;

            Renderer.BeginDraw(Camera.TransformationMatrix * Resolution.PreScaleTransform, currentRenderTarget);

            App.Graphics.FullViewportClear(Color.Transparent);

            for (int i = 0; i < _componentsToDraw.Count; i++)
            {
                _componentsToDraw[i].OnDraw();
            }

            Renderer.EndDraw();

            foreach (var effect in postProcessingEffects)
            {
                Renderer.BeginDraw(Matrix.Identity, effect.Target);

                App.Graphics.FullViewportClear(Color.Transparent);

                effect.OnDraw(currentRenderTarget, Camera.TransformationMatrix * Resolution.PreScaleTransform);

                Renderer.EndDraw();

                currentRenderTarget = effect.Target;
            }

            Renderer.BeginDraw(Camera.TransformationMatrix * Resolution.PreScaleTransform, currentRenderTarget);

            if (debug)
            {
                Partition.DebugDraw();

                for (int i = 0; i < _componentsToDraw.Count; i++)
                {
                    _componentsToDraw[i].OnDebugDraw();
                }
            }

            Renderer.EndDraw();

            return currentRenderTarget;
        }

        public virtual void End() 
        {
            RenderTarget.Dispose();

            foreach (var pp in postProcessingEffects) 
            {
                pp.Dispose();
            }

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnDestroy();
                _components[i].OnEnd();
            }

            content.Unload();
        }
    }
}
