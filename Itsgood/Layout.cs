using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{
    public class Layout
    {

        private readonly LayoutGrid _grid;
        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _toSpawn;
        private readonly List<WorldObject> _toDestroy;

        private IEnumerable<Behavior> _allBehaviors;

        public Layout(MainGame game)
        {
            Game = game;
            View = new LayoutView(game);

            _grid = new LayoutGrid(256, 4, 4);
            _worldObjects = new List<WorldObject>();
            _toSpawn = new List<WorldObject>();
            _toDestroy = new List<WorldObject>();
            _allBehaviors = new List<Behavior>();
        }

        public MainGame Game { get; }
        public LayoutView View { get; }

        public WorldObjectBuilder CreateObject(string imageFilePath, Rectangle source, Vector2 position, Rectangle bounds, Point origin)
        {
            var worldObject = new WorldObject(this)
            {
                ImageFilePath = imageFilePath,
                Source = source,
                Position = position,
                Color = Color.White,
                Bounds = bounds,
                Origin = origin
            };

            _toSpawn.Add(worldObject);

            return new WorldObjectBuilder(worldObject);
        }

        public WorldObjectBuilder CreateObject(Vector2 position, Rectangle bounds, Point origin)
        {
            var worldObject = new WorldObject(this)
            {
                Position = position,
                Color = Color.White,
                Bounds = bounds,
                Origin = origin
            };

            _toSpawn.Add(worldObject);

            return new WorldObjectBuilder(worldObject);
        }

        public void DestroyObject(WorldObject worldObject)
        {
            if (!worldObject.MarkedForDestroy)
            {
                worldObject.MarkForDestroy();

                _toDestroy.Add(worldObject);
            }
        }

        public bool TestOverlapOffset(WorldObject worldObject, float xOffset, float yOffset) 
        {
            return _grid.TestOverlapOffset(worldObject, xOffset, yOffset);
        }

        public bool TestOverlapOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            return _grid.TestOverlapOffset(worldObject, xOffset, yOffset, out overlappedWorldObject);
        }

        public IEnumerable<WorldObject> TestOverlap(Rectangle bounds) 
        {
            return _grid.TestOverlap(bounds);
        }

        internal void UpdatePosition(WorldObject worldObject)
        {
            _grid.Update(worldObject);
        }

        internal void Tick(GameTime gameTime)
        {
            InvokeBehaviorCallbacks();
            SyncWorldObjectLists();
            TickAllBehaviors(gameTime);
            SyncPreviousFrameData();
        }

        internal void Draw(SpriteBatch batch)
        {
            View.BeginDraw(batch);

            foreach (var worldObject in _worldObjects)
            {
                View.Draw(batch, worldObject);
            }

            View.EndDraw(batch);
        }

        private void InvokeBehaviorCallbacks()
        {
            foreach (var toSpawn in _toSpawn)
            {
                foreach (var behavior in toSpawn.GetBehaviors())
                {
                    behavior.Initialize();
                }
            }

            foreach (var toDestroy in _toDestroy)
            {
                foreach (var behavior in toDestroy.GetBehaviors())
                {
                    behavior.OnOwnerDestroyed();
                }
            }
        }

        private void SyncWorldObjectLists()
        {
            if (_toSpawn.Count == 0 && _toDestroy.Count == 0)
                return;

            foreach (var toSpawn in _toSpawn)
            {
                _worldObjects.Add(toSpawn);

                toSpawn.Load();
                _grid.Add(toSpawn);
                toSpawn.UpdateBBox();

                foreach (var behavior in toSpawn.GetBehaviors())
                {
                    behavior.OnOwnerCreated();
                }
            }

            _toSpawn.Clear();

            foreach (var toDestroy in _toDestroy)
            {
                _worldObjects.Remove(toDestroy);

                _grid.Remove(toDestroy);
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
