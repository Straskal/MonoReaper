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
        private readonly Input.PressedAction _toggleDebug;

        private Layout _nextLayout;
        private bool _isDebugging;

        internal MainGame(GameSettings gameSettings)
        {
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = gameSettings.IsResizable;
            Window.IsBorderless = !gameSettings.IsBordered;

            ViewportWidth = gameSettings.ViewportWidth;
            ViewportHeight = gameSettings.ViewportHeight;

            var input = new Input();
            _toggleDebug = input.NewPressedAction("toggleDebug");
            _toggleDebug.AddKey(Keys.OemTilde);

            Singletons = new SingletonList();
            Singletons.Register(input);

            _gpuManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = gameSettings.IsFullscreen,
                PreferredBackBufferWidth = ViewportWidth,
                PreferredBackBufferHeight = ViewportHeight,
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            _gpuManager.ApplyChanges();
        }

        public SingletonList Singletons { get; }
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
        }

        protected override void UnloadContent()
        {
            CurrentLayout.Unload();
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

        private void HandleLayoutChange() 
        {
            if (_nextLayout != null)
            {
                if (CurrentLayout != null)
                    CurrentLayout.Unload();

                CurrentLayout = _nextLayout;

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
