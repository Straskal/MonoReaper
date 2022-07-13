using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Core.Graphics;
using Reaper.Engine.Graphics;

namespace Core
{
    public class App : Game
    {
        public const string ContentRoot = "Content";

        public const int ResolutionWidth = 640;
        public const int ResolutionHeight = 200;

        public const bool StartFullscreen = true;

        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public static GraphicsDevice Graphics => GraphicsDeviceManager.GraphicsDevice;
        public static ContentManager ContentManager { get; private set; }
        public static int ViewportWidth { get; private set; }
        public static int ViewportHeight { get; private set; }
        public static float TotalTime { get; private set; }
        public static App Current { get; private set; }

        private bool _isDebugging;
        private Action _onChangeLevel;

        public App()
        {
            Current = this;

            Content.RootDirectory = ContentRoot;

            ViewportWidth = ResolutionWidth;
            ViewportHeight = ResolutionHeight;

            Window.AllowUserResizing = false;
            Window.IsBorderless = false;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = StartFullscreen,
                PreferredBackBufferWidth = StartFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : ViewportWidth,
                PreferredBackBufferHeight = StartFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : ViewportHeight,
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = true
            };

            GraphicsDeviceManager.ApplyChanges();

            Resolution.Width = ResolutionWidth;
            Resolution.Height = ResolutionHeight;
        }

        public Level CurrentLevel { get; private set; }

        public void ChangeLevel(Level level)
        {
            _onChangeLevel = () =>
            {
                CurrentLevel?.End();
                CurrentLevel = level;
                CurrentLevel?.Start();
                _onChangeLevel = null;
            };
        }

        public static void ToggleFullscreen()
        {
            GraphicsDeviceManager.ToggleFullScreen();

            if (GraphicsDeviceManager.IsFullScreen)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = ViewportWidth;
                GraphicsDeviceManager.PreferredBackBufferHeight = ViewportHeight;
            }

            GraphicsDeviceManager.ApplyChanges();
        }

        public void ToggleDebug()
        {
            _isDebugging = !_isDebugging;
        }

        protected override void LoadContent()
        {
            ContentManager = Content;
            Renderer.Initialize();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            if (CurrentLevel != null) 
            {
                CurrentLevel.End();
            }
            
            Renderer.Unload();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;

            Input.Poll();

            _onChangeLevel?.Invoke();

            CurrentLevel?.Tick(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (CurrentLevel != null) 
            {
                var processedRenderTarget = CurrentLevel.Draw(_isDebugging);

                Renderer.BeginDraw(Resolution.PostScaleTransform);
                Renderer.Draw(processedRenderTarget, Vector2.Zero, Color.White);
                Renderer.EndDraw();
            }
        }
    }
}
