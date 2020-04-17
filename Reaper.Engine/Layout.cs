using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Reaper.Engine
{
    /// <summary>
    /// Layouts contain world objects. Layouts can be used for menu screens, gameplay, etc...
    /// </summary>
    public sealed class Layout
    {
        public Layout(MainGame game, int cellSize, int width, int height)
        {
            Started = false;
            Game = game;
            Width = width;
            Height = height;
            Content = new ContentManager(game.Services, game.Content.RootDirectory);
            View = new LayoutView(game, this);
            Objects = new WorldObjectList(this);
            Grid = new WorldObjectGrid(cellSize, width, height);
        }

        public bool Started { get; private set; }

        /// <summary>
        /// The game...
        /// </summary>
        public MainGame Game { get; }

        /// <summary>
        /// The content for this layout. It will be unloaded when the layout ends.
        /// </summary>
        public ContentManager Content { get; }

        /// <summary>
        /// The layout's view, which is essentially the camera.
        /// </summary>
        public LayoutView View { get; }

        /// <summary>
        /// All of the world objects in the layout.
        /// </summary>
        public WorldObjectList Objects { get; }

        /// <summary>
        /// The grid representation of world space. The grid is used for spatial and overlap queries.
        /// </summary>
        public WorldObjectGrid Grid { get; }

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
            Content.Unload();
        }
    }
}
