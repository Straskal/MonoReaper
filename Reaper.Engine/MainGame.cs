using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _gpuManager;

        private Layout _nextLayout;

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
        }

        public SingletonList Singletons { get; private set; }
        public Layout CurrentLayout { get; private set; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public bool IsFullscreen => _gpuManager.IsFullScreen;

        /// <summary>
        /// Returns an empty layout to be filled with world objects.
        /// </summary>
        /// <param name="cellSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Layout GetEmptyLayout(int cellSize, int width, int height)
        {
            return new Layout(this, cellSize, width, height);
        }

        /// <summary>
        /// Set the new running layout of the game.
        /// </summary>
        /// <param name="layout"></param>
        public void ChangeLayout(Layout layout) 
        {
            _nextLayout = layout;
        }

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

        public void ToggleDebug()
        {
            _isDebugging = !_isDebugging;
        }

        protected override void UnloadContent()
        {
            CurrentLayout.End();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleLayoutChange();

            Singletons.Tick(gameTime);
            CurrentLayout.Tick(gameTime);
            CurrentLayout.PostTick(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CurrentLayout.Draw(_isDebugging);
            
            base.Draw(gameTime);
        }

        private void HandleLayoutChange() 
        {
            if (_nextLayout != null)
            {
                if (CurrentLayout != null)
                    CurrentLayout.End();

                CurrentLayout = _nextLayout;

                if (CurrentLayout != null)
                    CurrentLayout.Start();

                _nextLayout = null;
            }
        }
    }
}
