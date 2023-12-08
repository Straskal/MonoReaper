using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Graphics;

namespace Engine
{
    public class App : Game
    {
        public const string ContentRoot = "Content";
        public const bool StartFullscreen = false;
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public GraphicsDevice Graphics => GraphicsDeviceManager.GraphicsDevice;
        public static App Instance { get; private set; }

        private bool _isDebugging;
        private Action _onChangeLevel;
        public Action LoadInitialLevel { get; set; }

        public App(int targetResolutionWidth, int targetResolutionHeight, ResolutionScaleMode resolutionScaleMode)
        {
            Instance = this;
            ResolutionWidth = targetResolutionWidth;
            ResolutionHeight = targetResolutionHeight;

            Resolution.Width = targetResolutionWidth;
            Resolution.Height = targetResolutionHeight;
            Resolution.ScaleMode = resolutionScaleMode;

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
            Resolution.Initialize();
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
            Resolution.Update();
            Input.Update();
            _onChangeLevel?.Invoke();
            CurrentLevel?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (CurrentLevel == null) 
            {
                return;
            }

            Renderer.SetTarget(CurrentLevel.RenderTarget);
            Renderer.SetViewport(Resolution.CameraViewport);
            Renderer.Clear();
            Renderer.BeginDraw(CurrentLevel.Camera.TransformationMatrix);
            CurrentLevel.Draw(_isDebugging);
            Renderer.EndDraw();
            Renderer.SetTarget(null);
            Renderer.SetViewport(Resolution.RenderTargetViewport);
            Renderer.Clear();
            Renderer.BeginDraw(Resolution.RenderTargetUpscalingMatrix);
            Renderer.Draw(CurrentLevel.RenderTarget, Vector2.Zero);
            Renderer.EndDraw();
        }
    }
}
