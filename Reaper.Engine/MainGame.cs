using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine.Tools;
using System;

namespace Reaper.Engine
{
    public struct GameSettings 
    {
        public int ViewportWidth;
        public int ViewportHeight;
        public bool IsFullscreen;
        public bool IsResizable;
        public bool IsBordered;
        public bool IsVsyncEnabled;
    }

    public interface IGame : IDisposable
    {
        Layout RunningLayout { get; }
        int ViewportWidth { get; }
        int ViewportHeight { get; }
        bool IsFullscreen { get; }

        Layout GetEmptyLayout(int v, int width, int height);
        void ChangeLayout(Layout layout);
        void ToggleFullscreen();
        void Run();
    }

    public class MainGame : Game, IGame
    {
        private readonly GraphicsDeviceManager _gpuManager;

        private Layout _nextLayout;

        internal MainGame(GameSettings gameSettings)
        {
            _gpuManager = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = gameSettings.IsVsyncEnabled,
            };

            Content.RootDirectory = "Content";
            Window.AllowUserResizing = gameSettings.IsResizable;
            Window.IsBorderless = !gameSettings.IsBordered;

            ViewportWidth = gameSettings.ViewportWidth;
            ViewportHeight = gameSettings.ViewportHeight;

            _gpuManager.IsFullScreen = gameSettings.IsFullscreen;
            _gpuManager.PreferredBackBufferWidth = ViewportWidth;
            _gpuManager.PreferredBackBufferHeight = ViewportHeight;
            _gpuManager.ApplyChanges();
        }

        public Layout RunningLayout { get; private set; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public bool IsFullscreen => _gpuManager.IsFullScreen;

        public Layout GetEmptyLayout(int cellSize, int width, int height)
        {
            return new Layout(this, cellSize, width, height);
        }

        public void ChangeLayout(Layout layout) 
        {
            _nextLayout = layout;
        }

        public void ToggleFullscreen()
        {
            _gpuManager.ToggleFullScreen();
        }

        protected override void LoadContent()
        {
            DebugTools.Initialize(_gpuManager.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            RunningLayout.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextLayout != null) 
            {
                RunningLayout = _nextLayout;
                _nextLayout = null;
            }

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.F))
                ToggleFullscreen();

            DebugTools.Tick();
            Singletons.Tick(gameTime);
            RunningLayout.Tick(gameTime);
            RunningLayout.PostTick(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            RunningLayout.Draw();
            base.Draw(gameTime);
        }
    }
}
