using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// A data structure that holds all world objects in a layout.
    /// 
    /// Object creation is immediate. The moment an object is created, you can immediately access its properties.
    /// Object destruction is deferred to the end of the frame.
    /// </summary>
    public sealed class WorldObjectList
    {
        private readonly Layout _layout;
        private readonly ContentManager _content;
        private readonly List<WorldObject> _worldObjects;

        private WorldObject[] _worldObjectsThisFrame;
        private bool _started;

        public WorldObjectList(Layout layout, ContentManager content)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _worldObjects = new List<WorldObject>();
            _started = false;
        }

        /// <summary>
        /// Returns the first found behavior of the specified type or null if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetFirstBehaviorOfType<T>() where T : Behavior
        {
            return _worldObjects.FirstOrDefault(wo => wo.GetBehavior<T>() != null)?.GetBehavior<T>();
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

            if (_started)
            {
                worldObject.Load(_content);
                worldObject.OnCreated();
                worldObject.OnLayoutStarted();
            }
            return worldObject;
        }

        public void DestroyObject(WorldObject worldObject)
        {
            worldObject.MarkForDestroy();
        }

        internal void Start()
        {
            _started = true;
            var wosInLayoutStart = _worldObjects.ToArray();
            InvokeLoadOnAll(wosInLayoutStart);
            InvokeCreatedOnAll(wosInLayoutStart);
            InvokeLayoutStartedOnAll(wosInLayoutStart);
        }

        internal void FrameStart() 
        {
            _worldObjectsThisFrame = _worldObjects.ToArray();
        }

        internal void Tick(GameTime gameTime)
        {
            foreach (var wo in _worldObjectsThisFrame)
                wo.Tick(gameTime);
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var wo in _worldObjectsThisFrame)
                wo.PostTick(gameTime);
        }

        internal void FrameEnd() 
        {
            HandleDestroyedWorldObjects(_worldObjects);
            SortByZOrder(_worldObjects);
            SyncPreviousFrameData(_worldObjects);
        }

        internal void Draw(LayoutView view)
        {
            foreach (var worldObject in _worldObjects)
                worldObject.Draw(view);
        }

        internal void DebugDraw(LayoutView view)
        {
            foreach (var worldObject in _worldObjects)
                worldObject.DebugDraw(view);
        }

        private void InvokeLoadOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
                worldObject.Load(_content);
        }

        private void InvokeCreatedOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
                worldObject.OnCreated();
        }

        private void InvokeLayoutStartedOnAll(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
                worldObject.OnLayoutStarted();
        }
        private void HandleDestroyedWorldObjects(List<WorldObject> worldObjects) 
        {
            worldObjects.RemoveAll(wo =>
            {
                if (wo.MarkedForDestroy)
                {
                    wo.OnDestroyed();
                    return true;
                }
                return false;
            });
        }

        private void SortByZOrder(List<WorldObject> worldObjects) 
        {
            worldObjects.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
        }

        private void SyncPreviousFrameData(IEnumerable<WorldObject> worldObjects)
        {
            foreach (var worldObject in worldObjects)
                worldObject.UpdatePreviousPosition();
        }
    }
}
