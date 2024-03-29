﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// The world object list contains all world objects for a layout and is used for world object queries.
    /// </summary>
    public sealed class WorldObjectList : IEnumerable<WorldObject>
    {
        private readonly Layout _layout;
        private readonly List<WorldObject> _worldObjects;

        private WorldObject[] _worldObjectsThisFrame;

        public WorldObjectList(Layout layout)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _worldObjects = new List<WorldObject>();
        }

        /// <summary>
        /// Creates a new world object with the given definition and position.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public WorldObject Create(WorldObjectDefinition definition, Vector2 position)
        {
            var worldObject = new WorldObject(_layout) { Position = position };
            definition.Apply(worldObject);

            _worldObjects.Add(worldObject);
            _layout.Grid.Add(worldObject);

            // If the layout has already started, then just completely start and load newly spawned objects.
            // This way, behaviors that spawn objects can immediately access all properties of the spawned object.
            if (_layout.Started)
            {
                worldObject.Load();
                worldObject.OnCreated();
                worldObject.OnLayoutStarted();
            }

            return worldObject;
        }

        public void Destroy(WorldObject worldObject)
        {
            worldObject.MarkForDestroy();
        }

        public WorldObject FindFirstWithTag(string tag)
        {
            return _worldObjects.FirstOrDefault(wo => wo.Tags.Contains(tag));
        }

        public IEnumerable<WorldObject> FindWithTag(string tag)
        {
            return _worldObjects.Where(wo => wo.Tags.Contains(tag));
        }

        internal void Start()
        {
            // When we start the layout, we create a copy of all of the initial objects 
            // so we can safely iterate over it while other objects are being spawned into _worldObjects.
            var wosInLayoutStart = _worldObjects.ToArray();

            InvokeLoadOnAll(wosInLayoutStart);
            InvokeCreatedOnAll(wosInLayoutStart);
            InvokeLayoutStartedOnAll(wosInLayoutStart);
        }

        internal void FrameStart() 
        {
            RefreshCurrentList();
        }

        internal void Tick(GameTime gameTime)
        {
            foreach (var wo in _worldObjectsThisFrame)
            {
                wo.Tick(gameTime);
            }
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var wo in _worldObjectsThisFrame)
            {
                wo.PostTick(gameTime);
            }
        }

        internal void FrameEnd()
        {
            // Update previous frame data before processing destroyed objects. We need updated bounds and previous bounds.
            UpdatePreviousFrameData(_worldObjects);

            // Handling destroyed objects is deferred until the end of the frame, unlike spawning.
            HandleDestroyedWorldObjects(_worldObjects);

            // Sort last. Might want to consider only sorting when our list changes. This is just easier for now...
            SortByZOrder(_worldObjects);
        }

        internal void Draw(Renderer renderer)
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.Draw(renderer);
            }
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.DebugDraw(renderer);
            }
        }

        private void RefreshCurrentList() 
        {
            // We use a copy of the world object list so we can iterate while new objects are spawned into _worldObjects.
            _worldObjectsThisFrame = _worldObjects.ToArray();
        }

        private void InvokeLoadOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.Load();
            }
        }

        private void InvokeCreatedOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.OnCreated();
            }
        }

        private void InvokeLayoutStartedOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.OnLayoutStarted();
            }
        }
        
        private void HandleDestroyedWorldObjects(List<WorldObject> worldObjects) 
        {
            worldObjects.RemoveAll(wo =>
            {
                if (wo.IsDestroyed)
                {
                    wo.OnDestroyed();
                    _layout.Grid.Remove(wo);
                    return true;
                }
                return false;
            });
        }

        private void SortByZOrder(List<WorldObject> worldObjects) 
        {
            worldObjects.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
        }

        private void UpdatePreviousFrameData(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.UpdatePreviousFrameData();
            }
        }

        public IEnumerator<WorldObject> GetEnumerator()
        {
            foreach (var worldObject in _worldObjects) 
            {
                yield return worldObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
