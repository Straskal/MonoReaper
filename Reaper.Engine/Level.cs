using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine.Aabb;

namespace Reaper.Engine
{
    public class Level
    {
        private readonly ContentManager content; 

        private readonly List<Entity> entities = new();
        private readonly List<Entity> entitiesToDestroy = new();
        private readonly List<Component> components = new();
        private readonly List<Component> componentsToDraw = new();
        private readonly List<Component> componentsToRemove = new();

        private Action<Entity, Component> onComponentAdded;
        private Action<Entity, List<Component>> onComponentsAdded;

        public Level(int cellSize, int width, int height)
        {
            content = new ContentManager(App.Current.Services, App.ContentRoot);

            Width = width;
            Height = height;

            Camera = new Camera(this); 
            Partition = new SpatialPartition(cellSize, width, height);
        }

        public Camera Camera { get; }
        public SpatialPartition Partition { get; }

        public int Width { get; }
        public int Height { get; }

        public void Spawn(Entity entity)
        {
            entity.Level = this;

            entities.Add(entity);

            AddComponents(entity, entity.Components);
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            entity.Level = this;
            entity.Position = position;

            entities.Add(entity);

            AddComponents(entity, entity.Components);
        }

        public void Destroy(Entity entity)
        {
            if (!entity.IsDestroyed)
            {
                entity.IsDestroyed = true;
                entitiesToDestroy.Add(entity);
            }
        }

        internal void AddComponent(Entity entity, Component component) 
        {
            component.Entity = entity;
            component.OnAttach(entity);
            components.Add(component);
            onComponentAdded?.Invoke(entity, component);
        }

        internal void AddComponents(Entity entity, List<Component> components)
        {
            for (int i = 0; i < components.Count; i++) 
            {
                components[i].Entity = entity;
                components[i].OnAttach(entity);
            }

            this.components.AddRange(components);
            onComponentsAdded?.Invoke(entity, components);
        }

        internal void RemoveComponent(Component component) 
        {
            componentsToRemove.Add(component);
        }

        public virtual void Start() 
        {
            try
            {
                var copy = new List<Component>(components);

                onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                };

                onComponentsAdded = (e, c) =>
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        c[i].OnLoad(content);
                    }
                };

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnLoad(content);
                }

                onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                    c.OnSpawn();
                };

                onComponentsAdded = (e, c) =>
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

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnSpawn();
                }

                onComponentAdded = (e, c) =>
                {
                    c.OnLoad(content);
                    c.OnSpawn();
                    c.OnStart();

                    componentsToDraw.Add(c);
                };

                onComponentsAdded = (e, c) =>
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

                    componentsToDraw.AddRange(c);
                };

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnStart();
                }

                for (int i = 0; i < copy.Count; i++)
                {
                    copy[i].OnStart();
                }

                componentsToDraw.AddRange(copy);
            }
            catch (StackOverflowException) 
            {
                throw new InvalidOperationException("A component either spawns itself when it's initialized or there is a circular spawn thing going on..");
            }            
        }

        public virtual void Tick(GameTime gameTime)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnTick(gameTime);
            }

            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnPostTick(gameTime);
            }

            for (int i = 0; i < entitiesToDestroy.Count; i++)
            {
                var destroyedEntity = entitiesToDestroy[i];
                var destroyedComponents = destroyedEntity.Components;

                for (int j = 0; j < destroyedComponents.Count; j++)
                {
                    destroyedComponents[j].OnDestroy();
                }

                for (int j = 0; j < destroyedComponents.Count; j++)
                {
                    components.Remove(destroyedComponents[j]);
                    componentsToDraw.Remove(destroyedComponents[j]);
                    componentsToRemove.Remove(destroyedComponents[j]);
                }

                entities.Remove(destroyedEntity);
            }

            for (int i = 0; i < componentsToRemove.Count; i++) 
            {
                componentsToRemove[i].OnDetach();
                componentsToRemove[i].Entity = null;
            }

            entitiesToDestroy.Clear();
            componentsToRemove.Clear();
        }

        public virtual void Draw(bool debug)
        {
            componentsToDraw.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));

            for (int i = 0; i < componentsToDraw.Count; i++)
            {
                componentsToDraw[i].OnDraw();
            }

            if (debug)
            {
                Partition.DebugDraw();

                for (int i = 0; i < componentsToDraw.Count; i++)
                {
                    componentsToDraw[i].OnDebugDraw();
                }
            }
        }

        public virtual void End() 
        {
            content.Unload();
        }
    }
}
