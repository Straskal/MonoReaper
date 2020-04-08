using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine.Singletons;
using System;

namespace Reaper.Engine
{
    public interface IGame : IDisposable
    {
        SingletonList Singletons { get; }
        Layout CurrentLayout { get; }
        GameWindow Window { get; }
        int ViewportWidth { get; }
        int ViewportHeight { get; }
        bool IsFullscreen { get; }

        Layout GetEmptyLayout(int v, int width, int height);
        void ChangeLayout(Layout layout);
        void ToggleFullscreen();
        void Run();
        void Exit();
    }

    public class MainGame : Game, IGame
    {
        private readonly GraphicsDeviceManager _gpuManager;

        private Layout _nextLayout;

        // Debugging helper
        private Input.PressedAction _toggleDebug;
        private bool _isDebugging;

        internal MainGame(GameSettings gameSettings)
        {
            Content.RootDirectory = "Content";
            ViewportWidth = gameSettings.ViewportWidth;
            ViewportHeight = gameSettings.ViewportHeight;
            Window.AllowUserResizing = false;
            Window.IsBorderless = false;

            InitializeSingletons(); 
            
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

        protected override void UnloadContent()
        {
            CurrentLayout.End();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleDebugToggle();
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

        private void InitializeSingletons() 
        {
            var input = new Input();
            _toggleDebug = input.NewPressedAction("toggleDebug");
            _toggleDebug.AddKey(Keys.OemTilde);

            Singletons = new SingletonList();
            Singletons.Register(input);
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

        private void HandleDebugToggle() 
        {
            if (_toggleDebug.WasPressed())
                _isDebugging = !_isDebugging;
        }
    }
}
