using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// Layouts can range from boss battles, levels, to menu screens.
    /// 
    /// - Layouts contain world objects.
    /// - Layouts are mostly a pass through for their components (the view, spatial grid, and object lists, etc...).
    /// </summary>
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
            _grid = new LayoutGrid(cellSize, (int)Math.Ceiling((double)width / cellSize), (int)Math.Ceiling((double)height / cellSize));
            _worldObjectList = new WorldObjectList(this);

            Content = new ContentManager(game.Services, "Content");
        }

        public MainGame Game { get; }
        public ContentManager Content { get; }
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

        public T GetWorldObjectOfType<T>() where T : Behavior
        {
            return _worldObjectList.GetWorldObjectOfType<T>();
        }

        public bool TestOverlapSolidOffset(WorldObject worldObject, float xOffset, float yOffset) 
        {
            return _grid.TestSolidOverlapOffset(worldObject, xOffset, yOffset);
        }

        public bool TestOverlapSolidOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            return _grid.TestSolidOverlapOffset(worldObject, xOffset, yOffset, out overlappedWorldObject);
        }

        public WorldObject[] QueryBounds(Rectangle bounds) 
        {
            return _grid.QueryBounds(bounds);
        }

        internal void UpdateGridCell(WorldObject worldObject)
        {
            _grid.Update(worldObject);
        }

        internal void Destroy(WorldObject worldObject)
        {
            _grid.Remove(worldObject);
            _worldObjectList.DestroyObject(worldObject);
        }

        internal void Tick(GameTime gameTime)
        {
            _worldObjectList.Tick(gameTime);
        }

        internal void PostTick(GameTime gameTime)
        {
            _worldObjectList.PostTick(gameTime);
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
            Content.Unload();
        }
    }
}
