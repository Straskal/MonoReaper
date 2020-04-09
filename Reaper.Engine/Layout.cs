using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Reaper.Engine
{
    /// <summary>
    /// Layouts can range from boss battles, levels, to menu screens.
    /// 
    /// - Layouts have a view.
    /// - Layouts contain world objects.
    /// - Layouts store world objects in a grid for spatial queries.
    /// </summary>
    public sealed class Layout
    {
        private readonly ContentManager _content;

        public Layout(MainGame game, int cellSize, int width, int height)
        {
            _content = new ContentManager(game.Services, game.Content.RootDirectory);

            Game = game;
            Width = width;
            Height = height;
            View = new LayoutView(game, this);
            Grid = new LayoutGrid(cellSize, width, height);
            Objects = new WorldObjectList(this, _content);
        }

        public LayoutView View { get; }
        public LayoutGrid Grid { get; }
        public WorldObjectList Objects { get; }
        public MainGame Game { get; }
        public int Width { get; }
        public int Height { get; }

        public WorldObject Spawn(WorldObjectDefinition definition, Vector2 position)
        {
            var worldObject = Objects.Create(definition, position);
            Grid.Add(worldObject);
            return worldObject;
        }

        public void Destroy(WorldObject worldObject)
        {
            Grid.Remove(worldObject);
            Objects.DestroyObject(worldObject);
        }

        internal void Start() 
        {
            Objects.Start();
        }

        internal void Tick(GameTime gameTime)
        {
            Objects.FrameStart();
            Objects.Tick(gameTime);
        }

        internal void PostTick(GameTime gameTime)
        {
            Objects.PostTick(gameTime);
            Objects.FrameEnd();
        }

        internal void Draw(Renderer renderer, bool debug)
        {
            Objects.Draw(renderer);
            if (debug) Objects.DebugDraw(renderer);
        }

        internal void End() 
        {
            _content.Unload();
        }
    }
}
