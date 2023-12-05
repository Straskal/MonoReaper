using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Core.Collision;
using Core.Graphics;
using Reaper.Engine.Graphics;
using System.Linq;

namespace Core
{
    /// <summary>
    /// This class represents a level in the game.
    /// </summary>
    /// <remarks>
    /// A level contains entities and is responsible for orchestrating callback methods on entity components.
    /// </remarks>
    public class Level
    {
        /// <summary>
        /// The level's very own content manager.
        /// </summary>
        protected ContentManager Content { get; }

        /// <summary>
        /// WIP
        /// </summary>
        protected List<PostProcessingEffect> PostProcessingEffects { get; } = new();

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToDestroy = new();
        private readonly List<Component> _components = new();
        private readonly List<Component> _componentsToUpdate = new();
        private readonly List<Component> _componentsToDraw = new();
        private readonly List<Component> _componentsToRemove = new();

        private delegate void ComponentAddedHandler(Entity entity, params Component[] components);
        private ComponentAddedHandler _onComponentsAdded;

        public Level(int cellSize, int width, int height)
        {
            Content = new ContentManager(App.Current.Services, App.ContentRoot);
            Width = width;
            Height = height;
            Camera = new Camera(App.ViewportWidth, App.ViewportHeight);
            RenderTarget = new RenderTarget2D(App.GraphicsDeviceManager.GraphicsDevice, Resolution.RenderTargetWidth, Resolution.RenderTargetHeight);
            Partition = new Partition(cellSize);
        }

        /// <summary>
        /// Gets the level's camera.
        /// </summary>
        public Camera Camera { get; }

        /// <summary>
        /// Gets the level's render target.
        /// </summary>
        public RenderTarget2D RenderTarget { get; }

        /// <summary>
        /// Gets the level's spatial partition.
        /// </summary>
        public Partition Partition { get; }

        public int Width { get; }
        public int Height { get; }

        public void AddPostProcessingEffect(PostProcessingEffect effect)
        {
            PostProcessingEffects.Add(effect);
        }

        /// <summary>
        /// Spawns an entity at the given location.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        public void Spawn(Entity entity, Vector2 position)
        {
            if (entity.Level == null)
            {
                entity.Level = this;
                entity.Position = position;
                _entities.Add(entity);
                AddComponents(entity, entity.Components);
            }    
        }

        /// <summary>
        /// Destroys an entity and removes it from the level.
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>
        /// The entity destruction is deferred until the end of the current frame.
        /// </remarks>
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
            _onComponentsAdded?.Invoke(entity, component);
        }

        internal void AddComponents(Entity entity, List<Component> components)
        {
            foreach (var component in components)
            {
                component.Entity = entity;
            }
            _components.AddRange(components);
            _onComponentsAdded?.Invoke(entity, components.ToArray());
        }

        internal void RemoveComponent(Component component)
        {
            _componentsToRemove.Add(component);
        }

        /// <summary>
        /// Starts the level.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void Start()
        {
            try
            {
                var startComponents = _components;

                for (int i = 0; i < startComponents.Count; i++)
                {
                    startComponents[i].OnLoad(Content);
                }

                _onComponentsAdded = (entity, components) =>
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnLoad(Content);
                    }
                };

                for (int i = 0; i < startComponents.Count; i++)
                {
                    startComponents[i].OnSpawn();
                }

                _onComponentsAdded = (entity, components) =>
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnLoad(Content);
                    }

                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnSpawn();
                    }
                };

                for (int i = 0; i < startComponents.Count; i++)
                {
                    startComponents[i].OnStart();

                    if (startComponents[i].IsUpdateEnabled) 
                    {
                        _componentsToUpdate.Add(startComponents[i]);
                    }

                    if (startComponents[i].IsDrawEnabled)
                    {
                        _componentsToDraw.Add(startComponents[i]);
                    }
                }

                _onComponentsAdded = (entity, components) =>
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnLoad(Content);
                    }

                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnSpawn();
                    }

                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnStart();

                        if (components[i].IsUpdateEnabled)
                        {
                            _componentsToUpdate.Add(components[i]);
                        }

                        if (components[i].IsDrawEnabled)
                        {
                            _componentsToDraw.Add(components[i]);
                        }
                    }       
                };
            }
            catch (StackOverflowException)
            {
                throw new InvalidOperationException("A component either spawns itself when it's initialized or there is a circular spawn thing going on..");
            }
        }

        /// <summary>
        /// Updates the level and all of it's entities.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < _componentsToUpdate.Count; i++)
            {
                _componentsToUpdate[i].OnUpdate(gameTime);
            }

            for (int i = 0; i < _componentsToUpdate.Count; i++)
            {
                _componentsToUpdate[i].OnPostUpdate(gameTime);
            }

            PostUpdateDestroyEntities();
            PostUpdateRemoveComponents();

            foreach (var effect in PostProcessingEffects)
            {
                effect.OnUpdate(gameTime);
            }
        }

        private void PostUpdateDestroyEntities()
        {
            for (int i = 0; i < _entitiesToDestroy.Count; i++)
            {
                for (int j = 0; j < _entitiesToDestroy[i].Components.Count; j++)
                {
                    _entitiesToDestroy[i].Components[j].OnDestroy();
                }

                for (int j = 0; j < _entitiesToDestroy[i].Components.Count; j++)
                {
                    _components.Remove(_entitiesToDestroy[i].Components[j]);
                    _componentsToUpdate.Remove(_entitiesToDestroy[i].Components[j]);
                    _componentsToDraw.Remove(_entitiesToDestroy[i].Components[j]);
                    _componentsToRemove.Remove(_entitiesToDestroy[i].Components[j]);
                }

                _entities.Remove(_entitiesToDestroy[i]);
            }

            _entitiesToDestroy.Clear();
        }

        private void PostUpdateRemoveComponents()
        {
            for (int i = 0; i < _componentsToRemove.Count; i++)
            {
                _componentsToRemove[i].OnDestroy();
                _componentsToRemove[i].Entity = null;
                _components.Remove(_componentsToRemove[i]);
                _componentsToUpdate.Remove(_componentsToRemove[i]);
                _componentsToDraw.Remove(_componentsToRemove[i]);
            }

            _componentsToRemove.Clear();
        }

        public virtual RenderTarget2D Draw(bool debug)
        {
            _components.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
            var currentRenderTarget = RenderTarget;
            Renderer.BeginDraw(Camera.TransformationMatrix, currentRenderTarget);
            App.Graphics.FullViewportClear(Color.Transparent);

            for (int i = 0; i < _componentsToDraw.Count; i++)
            {
                _componentsToDraw[i].OnDraw();
            }

            Renderer.EndDraw();

            //foreach (var effect in PostProcessingEffects)
            //{
            //    Renderer.BeginDraw(Matrix.Identity, effect.Target);

            //    App.Graphics.FullViewportClear(Color.Transparent);

            //    effect.OnDraw(currentRenderTarget, Camera.TransformationMatrix);

            //    Renderer.EndDraw();

            //    currentRenderTarget = effect.Target;
            //}

            if (debug) 
            {
                Renderer.BeginDraw(Camera.TransformationMatrix, currentRenderTarget);

                Partition.DebugDraw();

                for (int i = 0; i < _components.Count; i++)
                {
                    _components[i].OnDebugDraw();
                }

                Renderer.EndDraw();
            }



            return currentRenderTarget;
        }

        public virtual void End()
        {
            RenderTarget.Dispose();

            foreach (var effect in PostProcessingEffects)
            {
                effect.Dispose();
            }

            var allComponents = _entities.SelectMany(entity => entity.Components);

            foreach (var component in allComponents) 
            {
                component.OnEnd();
            }

            Content.Unload();
        }
    }
}
