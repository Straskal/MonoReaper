using Microsoft.Xna.Framework;
using Reaper.Engine.Singletons;
using Reaper.Engine.Tools;
using System;

namespace Reaper.Engine
{
    public interface IGame : IDisposable
    {
        SingletonList Singletons { get; }
        Layout CurrentLayout { get; }
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

            Singletons = new SingletonList();
            Singletons.Register(new Input());
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

        protected override void LoadContent()
        {
            DebugTools.Initialize(_gpuManager.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            CurrentLayout.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleLayoutChange();

            DebugTools.Tick();
            Singletons.Tick(gameTime);
            CurrentLayout.Tick(gameTime);
            CurrentLayout.PostTick(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CurrentLayout.Draw();
            
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
    }
}
