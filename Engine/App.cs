using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Graphics;
using Engine.Extensions;

namespace Engine
{
    public class App : Game
    {
        public const string ContentRoot = "Content";
        public const bool StartFullscreen = false;
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public static GraphicsDevice Graphics => GraphicsDeviceManager.GraphicsDevice;
        public static float TotalTime { get; private set; }
        public static App Instance { get; private set; }

        private bool _isDebugging;
        private Action _onChangeLevel;
        public Action LoadInitialLevel { get; set; }

        public App(int targetResolutionWidth, int targetResolutionHeight)
        {
            ResolutionWidth = targetResolutionWidth;
            ResolutionHeight = targetResolutionHeight;

            Instance = this;
            Content = new ContentManagerExtended(Services, ContentRoot);
            Window.AllowUserResizing = true;
            Window.IsBorderless = false;
            IsMouseVisible = true;
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = StartFullscreen,
                PreferredBackBufferWidth = StartFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : ResolutionWidth,
                PreferredBackBufferHeight = StartFullscreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : ResolutionHeight,
                HardwareModeSwitch = false
            };
            GraphicsDeviceManager.ApplyChanges();
        }

        public int ResolutionWidth { get; }
        public int ResolutionHeight { get; }
        public Random Random { get; } = new();

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

        public void ToggleFullscreen()
        {
            GraphicsDeviceManager.ToggleFullScreen();

            if (GraphicsDeviceManager.IsFullScreen)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
                GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
            }

            GraphicsDeviceManager.ApplyChanges();
        }

        public void ToggleDebug()
        {
            _isDebugging = !_isDebugging;
        }

        protected override void LoadContent()
        {
            Resolution.Initialize(ResolutionWidth, ResolutionHeight);
            Renderer.Initialize(GraphicsDevice);
            LoadInitialLevel?.Invoke();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            CurrentLevel?.End();
            Renderer.Deinitialize();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            Resolution.Update();
            Input.Update();
            _onChangeLevel?.Invoke();
            CurrentLevel?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetRenderTarget(CurrentLevel?.RenderTarget);
            Renderer.LetterboxClear();
            CurrentLevel?.Draw(_isDebugging);
            Renderer.SetRenderTarget(null);
            Renderer.BeginDraw(Resolution.RenderTargetUpscalingMatrix);
            Renderer.Draw(CurrentLevel?.RenderTarget, Vector2.Zero);
            Renderer.EndDraw();
        }
    }
}
