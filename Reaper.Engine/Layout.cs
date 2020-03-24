using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine.Tools;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    /// <summary>
    /// Layouts can range from boss battles, levels, to menu screens.
    /// 
    /// - Layouts contain world objects.
    /// - Layouts are mostly a pass through for their components (the view, spatial grid, and object lists, etc...).
    /// </summary>
    public sealed class Layout
    {
        private readonly ContentManager _content;

        public Layout(MainGame game, int cellSize, int width, int height)
        {
            _content = new ContentManager(game.Services, "Content");

            Game = game;
            Width = width;
            Height = height;
            View = new LayoutView(game, this);

            // TODO: This conversion should probably be done by the grid.
            Grid = new LayoutGrid(cellSize, (int)Math.Ceiling((double)width / cellSize), (int)Math.Ceiling((double)height / cellSize));
            Objects = new WorldObjectList(this, _content);
        }

        public LayoutView View { get; }
        public LayoutGrid Grid { get; }
        public WorldObjectList Objects { get; }
        public IGame Game { get; }

        public int Width { get; }
        public int Height { get; }

        public WorldObject Spawn(WorldObjectDefinition definition, Vector2 position)
        {
            var worldObject = Objects.Create(definition, position);
            worldObject.UpdateBBox();

            Grid.Add(worldObject);

            return worldObject;
        }

        public void Destroy(WorldObject worldObject)
        {
            Grid.Remove(worldObject);
            Objects.DestroyObject(worldObject);
        }

        internal void Tick(GameTime gameTime)
        {
            Objects.Tick(gameTime);
        }

        internal void PostTick(GameTime gameTime)
        {
            Objects.PostTick(gameTime);
        }

        internal void Draw()
        {
            View.BeginDraw();
            Objects.Draw(View);

            DebugTools.Draw(View.SpriteBatch, Objects.WorldObjects);

            View.EndDraw();
        }

        internal void Unload() 
        {
            View.Unload();
            _content.Unload();
        }
    }
}
