using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Graphics;
using Engine.Collision;

namespace Engine
{
    /// <summary>
    /// This class represents a level in the game, which handles tracking entities and components.
    /// </summary>
    public class Level : Screen
    {
        public enum LoadStatus
        {
            NotStarted,
            Loading,
            Loaded
        }

        /// <summary>
        /// A content manager for level specific content. The content will be unloaded when the level is ended.
        /// </summary>
        protected ContentManagerExtended Content
        {
            get;
            private set;
        }

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _entitiesToDestroy = new();
        private readonly List<Component> _components = new();
        private readonly List<Component> _componentsToRemove = new();

        private delegate void ComponentAddedHandler(Entity entity, params Component[] components);
        private ComponentAddedHandler _onComponentsAdded;
        private bool _sortComponents = false;
        private Coroutine _loadCoroutine;

        public Level(App application, int cellSize, int width, int height) : base(application)
        {
            Content = new ContentManagerExtended(application.Services, application.Content.RootDirectory);
            Width = width;
            Height = height;
            Camera = new Camera(application.BackBuffer);
            Partition = new Partition(cellSize);
        }

        /// <summary>
        /// Gets the level's camera.
        /// </summary>
        public Camera Camera
        {
            get;
        }

        /// <summary>
        /// Gets the level's spatial partition.
        /// </summary>
        public Partition Partition
        {
            get;
        }

        /// <summary>
        /// Gets the level's width in pixels
        /// </summary>
        public int Width
        {
            get;
        }

        /// <summary>
        /// Gets the level's height in pixels
        /// </summary>
        public int Height
        {
            get;
        }

        /// <summary>
        /// Gets the level's loading status
        /// </summary>
        public LoadStatus Status
        {
            get;
            private set;
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
        /// Creates a new entity for the given component and spawns it
        /// </summary>
        /// <param name="component"></param>
        /// <param name="position"></param>
        public void Spawn(Component component, Vector2 position)
        {
            if (component.Entity == null)
            {
                var entity = new Entity(Origin.Center);
                entity.AddComponent(component);
                Spawn(entity, position);
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

        /// <summary>
        /// Adds the given component to the level
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        internal void AddComponent(Entity entity, Component component)
        {
            component.Entity = entity;
            _components.Add(component);
            _onComponentsAdded?.Invoke(entity, component);
            _sortComponents = true;
        }

        /// <summary>
        /// Adds the given components to the level
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="components"></param>
        internal void AddComponents(Entity entity, List<Component> components)
        {
            foreach (var component in components)
            {
                component.Entity = entity;
            }
            _components.AddRange(components);
            _onComponentsAdded?.Invoke(entity, components.ToArray());
            _sortComponents = true;
        }

        /// <summary>
        /// Removes the given component from the level
        /// </summary>
        /// <param name="component"></param>
        internal void RemoveComponent(Component component)
        {
            _componentsToRemove.Add(component);
        }

        public override void Start()
        {
            if (Status == LoadStatus.NotStarted)
            {
                _loadCoroutine = Application.Coroutines.Start(LoadRoutine());
            }
        }

        public override void Stop()
        {
            Application.Coroutines.Stop(_loadCoroutine);
            EndComponents();
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateComponents(gameTime);
            PostUpdateComponents(gameTime);
            ProcessDestroyedEntities();
            ProcessRemovedComponents();
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            SortComponentsIfNeeded();
            renderer.BeginDraw(Camera.TransformationMatrix);
            DrawComponents(renderer, gameTime);
            renderer.EndDraw();
        }

        private IEnumerator LoadRoutine()
        {
            Status = LoadStatus.Loading;
            yield return LoadComponentsRoutine();
            SpawnComponents();
            StartComponents();
            Status = LoadStatus.Loaded;
        }

        private IEnumerator LoadComponentsRoutine()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnLoad(Content);
                yield return null;
            }

            _onComponentsAdded = (entity, components) =>
            {
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].OnLoad(Content);
                }
            };
        }

        private void SpawnComponents()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnSpawn();
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
        }

        private void StartComponents()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnStart();
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
                }
            };
        }

        private void UpdateComponents(GameTime gameTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnUpdate(gameTime);
            }
        }

        private void PostUpdateComponents(GameTime gameTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnPostUpdate(gameTime);
            }
        }

        private void DrawComponents(Renderer renderer, GameTime gameTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnDraw(renderer, gameTime);
            }
        }

        private void EndComponents()
        {
            var components = new List<Component>();
            foreach (var entity in _entities)
            {
                components.AddRange(entity.Components);
            }

            foreach (var component in components)
            {
                component.OnEnd();
            }
        }

        private void ProcessDestroyedEntities()
        {
            for (int i = 0; i < _entitiesToDestroy.Count; i++)
            {
                var entity = _entitiesToDestroy[i];
                var components = entity.Components;

                for (int j = 0; j < components.Count; j++)
                {
                    components[j].OnDestroy();
                }

                for (int j = 0; j < components.Count; j++)
                {
                    components[j].OnEnd();
                }

                for (int j = 0; j < components.Count; j++)
                {
                    components[j].Entity = null;
                    _components.Remove(components[j]);
                    _componentsToRemove.Remove(components[j]);
                }

                _entities.Remove(entity);
            }

            _entitiesToDestroy.Clear();
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
            if (_sortComponents)
            {
                _components.Sort(SortComponentsByZOrder);
                _sortComponents = false;
            }
        }

        private static int SortComponentsByZOrder(Component a, Component b)
        {
            return Comparer<int>.Default.Compare(a.ZOrder, b.ZOrder);
        }
    }
}
