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
    internal sealed class WorldObjectList
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

        public List<WorldObject> WorldObjects => _worldObjects;

        /// <summary>
        /// Returns the first found behavior of the specified type or null if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWorldObjectOfType<T>() where T : Behavior
        {
            return _worldObjects.FirstOrDefault(wo => wo.GetBehavior<T>() != null)?.GetBehavior<T>();
        }

        public WorldObject Create(WorldObjectDefinition definition, Vector2 position)
        {
            var worldObject = new WorldObject(_layout)
            {
                Position = position
            };

            definition.Build(worldObject);
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
            var toSpawn = _toSpawn.ToArray();
            var toDestroy = _toDestroy.ToArray();

            _toSpawn.Clear();
            _toDestroy.Clear();

            InvokeLoad(toSpawn);
            InvokeCreated(toSpawn);
            InvokeStarted(toSpawn);
            InvokeDestroyed(toDestroy);

            TickWorldObjects(gameTime);
            ZOrderSort();
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
        }

        private void InvokeLoad(IEnumerable<WorldObject> worldObjects) 
        {
            foreach (var worldObject in worldObjects)
            {
                _worldObjects.Add(worldObject);

                worldObject.Load(_content);
            }
        }

        private void InvokeCreated(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.OnCreated();
            }
        }

        private void InvokeStarted(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.OnLayoutStarted();
            }
        }

        private void InvokeDestroyed(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
            {
                worldObject.Destroy();
            }
        }

        private void ZOrderSort() 
        {
            _worldObjects.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
        }

        private void TickWorldObjects(GameTime gameTime) 
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
