using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        void ChangeState(MainGameState state);
        void ToggleFullscreen();
        void Run();
        void Exit();
    }

    public class MainGame : Game, IGame
    {
        private readonly GraphicsDeviceManager _gpuManager;

        private MainGameState _currentState;
        private MainGameState _nextState;
        private Layout _nextLayout;

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
                SynchronizeWithVerticalRetrace = true,
                HardwareModeSwitch = false,
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

        public void ChangeState(MainGameState state) 
        {
            _nextState = state;
            _nextState.Game = this;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            CurrentLayout.Unload();
            Content.Unload();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleLayoutChange();
            HandleStateChange();
            TickCurrentState(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawCurrentState(gameTime);

            base.Draw(gameTime);
        }

        private void InitializeSingletons() 
        {
            Singletons = new SingletonList();
            Singletons.Register(new Input());
        }

        private void HandleStateChange()
        {
            if (_nextState != null)
            {
                if (_currentState != null)
                    _nextState.End();

                _currentState = _nextState;
                _currentState.Start();
                _nextState = null;
            }
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

        private void TickCurrentState(GameTime gameTime) 
        {
            _currentState.Tick(gameTime);
        }

        private void DrawCurrentState(GameTime gameTime)
        {
            _currentState.Draw(gameTime);
        }
    }
}
