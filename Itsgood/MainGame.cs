using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ItsGood
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

        void Run();
        void ToBlankLayout();
        void ToggleFullscreen();
    }

    public class MainGame : Game, IGame
    {
        private readonly GraphicsDeviceManager _gpuManager;

        private SpriteBatch _spriteBatch;

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

            ToBlankLayout();
        }

        public Layout RunningLayout { get; private set; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public bool IsFullscreen => _gpuManager.IsFullScreen;

        public void ToBlankLayout() 
        {
            RunningLayout = new Layout(this);
        }

        public void ToggleFullscreen()
        {
            _gpuManager.ToggleFullScreen();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            
        }
        
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.F))
                ToggleFullscreen();

            RunningLayout.Tick(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            RunningLayout.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}
