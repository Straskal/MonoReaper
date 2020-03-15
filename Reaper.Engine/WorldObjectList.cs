using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    internal class WorldObjectList
    {
        private readonly Layout _layout;
        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _toSpawn;
        private readonly List<WorldObject> _toDestroy;

        private IEnumerable<Behavior> _allBehaviors;

        public WorldObjectList(Layout layout) 
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));

            _worldObjects = new List<WorldObject>();
            _toSpawn = new List<WorldObject>();
            _toDestroy = new List<WorldObject>();
            _allBehaviors = new List<Behavior>();
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
            InvokeBehaviorCallbacks();
            SyncWorldObjectLists();
            TickAllBehaviors(gameTime);
            SyncPreviousFrameData();
        }

        public void Draw(LayoutView view) 
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.Draw(view);
            }
        }

        private void InvokeBehaviorCallbacks()
        {
            for (int i = 0; i < _toSpawn.Count; i++) 
            {
                _toSpawn[i].Load(_layout.Game.Content);
            }

            foreach (var toDestroy in _toDestroy)
            {
                toDestroy.OnDestroyed();
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

            _toSpawn.Clear();

            foreach (var toDestroy in _toDestroy)
            {
                toDestroy.OnDestroyed();

                _worldObjects.Remove(toDestroy);
            }

            _toDestroy.Clear();

            _allBehaviors = _worldObjects.SelectMany(worldObject => worldObject.GetBehaviors());
        }

        private void SyncPreviousFrameData()
        {
            foreach (var worldObject in _worldObjects)
            {
                worldObject.UpdatePreviousPosition();
            }
        }

        private void TickAllBehaviors(GameTime gameTime)
        {
            foreach (var behavior in _allBehaviors)
            {
                behavior.Tick(gameTime);
            }
        }
    }
}
