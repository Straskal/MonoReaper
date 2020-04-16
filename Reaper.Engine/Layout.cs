using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Reaper.Engine
{
    /// <summary>
    /// Layouts can range from boss battles, levels, to menu screens.
    /// 
    /// - Layouts have a view.
    /// - Layouts contain world objects.
    /// - Layouts have a grid representation of world space for object queries.
    /// </summary>
    public sealed class Layout
    {
        private readonly ContentManager _content;

        public Layout(MainGame game, int cellSize, int width, int height)
        {
            _content = new ContentManager(game.Services, game.Content.RootDirectory);

            Started = false;
            Game = game;
            Width = width;
            Height = height;
            View = new LayoutView(game, this);
            Grid = new WorldObjectGrid(cellSize, width, height);
            Objects = new WorldObjectList(this, _content);
        }

        public bool Started { get; private set; }

        /// <summary>
        /// The layout's view, which is essentially the camera.
        /// </summary>
        public LayoutView View { get; }

        /// <summary>
        /// The grid representation of world space. The grid is used for spatial and overlap queries.
        /// </summary>
        public WorldObjectGrid Grid { get; }

        /// <summary>
        /// All of the world objects in the layout.
        /// </summary>
        public WorldObjectList Objects { get; }

        /// <summary>
        /// The game...
        /// </summary>
        public MainGame Game { get; }

        /// <summary>
        /// The width of the layout in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the layout in pixels.
        /// </summary>
        public int Height { get; }

        internal void Start() 
        {
            Started = true;
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

            if (debug) 
            {
                Grid.DebugDraw(renderer);
                Objects.DebugDraw(renderer);
            }
        }

        internal void End() 
        {
            _content.Unload();
        }
    }
}
