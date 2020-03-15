using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItsGood
{
    public class Layout
    {
        private readonly LayoutView _view;
        private readonly LayoutGrid _grid;
        private readonly WorldObjectList _worldObjectList;

        public Layout(MainGame game)
        {
            Game = game;

            _view = new LayoutView(game);
            _grid = new LayoutGrid(256, 4, 4);
            _worldObjectList = new WorldObjectList(this);
        }

        public MainGame Game { get; }

        public Vector2 Position
        {
            get => _view.Position;
            set => _view.Position = value;
        }

        public float Zoom 
        {
            get => _view.Zoom;
            set => _view.Zoom = value;
        }

        public WorldObject Spawn(WorldObjectDefinition definition, Vector2 position)
        {
            var worldObject = _worldObjectList.Create(position);
            definition.Build(worldObject);

            _grid.Add(worldObject);
            return worldObject;
        }

        public bool TestOverlapOffset(WorldObject worldObject, float xOffset, float yOffset) 
        {
            return _grid.TestOverlapOffset(worldObject, xOffset, yOffset);
        }

        public bool TestOverlapOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            return _grid.TestOverlapOffset(worldObject, xOffset, yOffset, out overlappedWorldObject);
        }

        public WorldObject[] TestOverlap(Rectangle bounds) 
        {
            return _grid.TestOverlap(bounds);
        }
               
        internal void Destroy(WorldObject worldObject)
        {
            _grid.Remove(worldObject);
            _worldObjectList.DestroyObject(worldObject);
        }

        internal void UpdateGridCell(WorldObject worldObject)
        {
            _grid.Update(worldObject);
        }

        internal void Tick(GameTime gameTime)
        {
            _worldObjectList.Tick(gameTime);
        }

        internal void Draw(SpriteBatch batch)
        {
            _view.BeginDraw(batch);
            _worldObjectList.Draw(_view);
            _view.EndDraw();
        }
    }
}
