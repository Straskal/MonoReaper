using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// A data structure that holds all world objects in a layout.
    /// </summary>
    internal class WorldObjectList
    {
        private readonly Layout _layout;
        private readonly ContentManager _content;
        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _toSpawn;
        private readonly List<WorldObject> _toDestroy;

        public WorldObjectList(Layout layout, ContentManager content) 
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _content = content ?? throw new ArgumentNullException(nameof(content));

            _worldObjects = new List<WorldObject>();
            _toSpawn = new List<WorldObject>();
            _toDestroy = new List<WorldObject>();
        }

        /// <summary>
        /// Returns the first found behavior of the specified type or null if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWorldObjectOfType<T>() where T : Behavior
        {
            return _worldObjects.FirstOrDefault(wo => wo.GetBehavior<T>() != null)?.GetBehavior<T>();
        }

        public WorldObject Create(Vector2 position)
        {
            var worldObject = new WorldObject(_layout)
            {
                Position = position
            };

            _toSpawn.Add(worldObject);

            return worldObject;
        }

        public void DestroyObject(WorldObject worldObject)
        {
            if (!worldObject.MarkedForDestroy)
            {
                worldObject.MarkForDestroy();

                _toDestroy.Add(worldObject);
            }
        }

        public void Tick(GameTime gameTime)
        {
            LoadSpawnedWorldObjects();
            SyncWorldObjectLists();
            TickBehaviors(gameTime);
            SyncPreviousFrameData();
        }

        public void PostTick(GameTime gameTime)
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.PostTick(gameTime);
            }
        }

        public void Draw(LayoutView view) 
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.Draw(view);
            }

            DebugTools.DrawBounds(view.SpriteBatch, _worldObjects);
        }

        private void LoadSpawnedWorldObjects()
        {
            for (int i = 0; i < _toSpawn.Count; i++) 
            {
                _toSpawn[i].Load(_content);
            }
        }

        private void SyncWorldObjectLists()
        {
            if (_toSpawn.Count == 0 && _toDestroy.Count == 0)
                return;

            foreach (var toSpawn in _toSpawn)
            {
                _worldObjects.Add(toSpawn);

                toSpawn.UpdateBBox();
                toSpawn.OnCreated();
            }

            foreach (var toSpawn in _toSpawn)
            {
                toSpawn.OnLayoutStarted();
            }

            _toSpawn.Clear();

            foreach (var toDestroy in _toDestroy)
            {
                toDestroy.OnDestroyed();

                _worldObjects.Remove(toDestroy);
            }

            _toDestroy.Clear();

            _worldObjects.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
        }

        private void TickBehaviors(GameTime gameTime) 
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.Tick(gameTime);
            }
        }

        private void SyncPreviousFrameData()
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.UpdatePreviousPosition();
            }
        }
    }
}
