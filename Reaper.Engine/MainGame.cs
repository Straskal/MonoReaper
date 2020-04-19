using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _gpuManager;
        private readonly Renderer _renderer;

        private Layout _nextLayout;
        private bool _paused;

        // Debugging helper
        private bool _isDebugging;

        public MainGame(GameSettings gameSettings)
        {
            Content.RootDirectory = "Content";
            ViewportWidth = gameSettings.ViewportWidth;
            ViewportHeight = gameSettings.ViewportHeight;
            Window.AllowUserResizing = false;
            Window.IsBorderless = false;
            Singletons = new SingletonList();

            _gpuManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = gameSettings.IsFullscreen,
                PreferredBackBufferWidth = gameSettings.IsFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : ViewportWidth,
                PreferredBackBufferHeight = gameSettings.IsFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : ViewportHeight,
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            _gpuManager.ApplyChanges();
            _renderer = new Renderer(this);
        }

        public SingletonList Singletons { get; private set; }
        public Layout CurrentLayout { get; private set; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public float TotalTime { get; private set; }

        /// <summary>
        /// Set the new running layout of the game.
        /// Layouts can be created using their constructor.
        /// This allows the game code to load layouts any way they like.
        /// </summary>
        /// <param name="layout"></param>
        public void ChangeLayout(Layout layout) 
        {
            _nextLayout = layout;
        }

        /// <summary>
        /// Toggle the game's fullscreen on and off.
        /// </summary>
        public void ToggleFullscreen()
        {
            _gpuManager.ToggleFullScreen();

            if (_gpuManager.IsFullScreen)
            {
                _gpuManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _gpuManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _gpuManager.PreferredBackBufferWidth = ViewportWidth;
                _gpuManager.PreferredBackBufferHeight = ViewportHeight;
            }

            _gpuManager.ApplyChanges();
        }

        /// <summary>
        /// Toggle paused for the entire game. When the game is paused, the only callback received by singletons and behaviors is HandleInput().
        /// </summary>
        public void TogglePaused() 
        {
            _paused = !_paused;
        }

        /// <summary>
        /// Toggle the game's debug tools on and off.
        /// </summary>
        public void ToggleDebug()
        {
            _isDebugging = !_isDebugging;
        }

        protected override void LoadContent()
        {
            Singletons.Load();
        }

        protected override void UnloadContent()
        {
            CurrentLayout.End();
            _renderer.Unload();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            Singletons.HandleInput(gameTime);

            if (!_paused) 
            {
                HandleLayoutChange();

                Singletons.Tick(gameTime);
                CurrentLayout.Tick(gameTime);
                CurrentLayout.PostTick(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _renderer.BeginDraw();
            CurrentLayout.Draw(_renderer, _isDebugging);
            Singletons.Draw(_renderer, _isDebugging);
            Singletons.DrawGUI(_renderer);
            _renderer.EndDraw();
            
            base.Draw(gameTime);
        }

        private void HandleLayoutChange() 
        {
            if (_nextLayout != null)
            {
                if (CurrentLayout != null)
                {
                    CurrentLayout.End();
                    Singletons.OnLayoutEnded();
                }

                CurrentLayout = _nextLayout;

                if (CurrentLayout != null)
                {
                    CurrentLayout.Start();
                    Singletons.OnLayoutStarted();
                }

                _nextLayout = null;
            }
        }
    }
}
