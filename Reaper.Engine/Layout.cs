using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    public class Layout
    {
        private readonly LayoutView _view;
        private readonly LayoutGrid _grid;
        private readonly WorldObjectList _worldObjectList;

        public Layout(MainGame game, int cellSize, int width, int height)
        {
            Game = game;
            Width = width;
            Height = height;

            _view = new LayoutView(game, this);
            _grid = new LayoutGrid(cellSize * 10, (int)Math.Ceiling((double)width / cellSize * 10), (int)Math.Ceiling((double)height / cellSize * 10));
            _worldObjectList = new WorldObjectList(this);
        }

        public MainGame Game { get; }

        public int Width { get; }
        public int Height { get; }

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

        internal void Draw()
        {
            _view.BeginDraw();
            _worldObjectList.Draw(_view);
            _view.EndDraw();
        }

        internal void Unload() 
        {
            _view.Unload();
        }
    }
}
