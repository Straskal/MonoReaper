using System;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
    /// <summary>
    /// The main runner for the game.
    /// </summary>
    public class App : Game
    {
        /// <summary>
        /// Gets the single instance of App
        /// </summary>
        public static App Instance { get; private set; }

        public App(int targetResolutionWidth, int targetResolutionHeight, ResolutionScaleMode resolutionScaleMode)
        {
            Instance = this;
            ResolutionWidth = targetResolutionWidth;
            ResolutionHeight = targetResolutionHeight;
            ResolutionScaleMode = resolutionScaleMode;
            Content = new ContentManagerExtended(Services, "Content");

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;

#if DEBUG
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
            IsMouseVisible = true;
#else
            GraphicsDeviceManager.IsFullScreen = true;
            IsMouseVisible = false;
#endif
        }

        /// <summary>
        /// Gets the target resolution width
        /// </summary>
        public int ResolutionWidth
        {
            get;
        }

        /// <summary>
        /// Gets the target resolution height
        /// </summary>
        public int ResolutionHeight
        {
            get;
        }

        public ResolutionScaleMode ResolutionScaleMode 
        {
            get;
        }

        /// <summary>
        /// Gets the graphics device manager
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get;
        }

        /// <summary>
        /// Gets the game state stack
        /// </summary>
        public ScreenStack Screens
        {
            get;
        } = new();

        /// <summary>
        /// Gets the coroutine runner
        /// </summary>
        public CoroutineRunner Coroutines
        {
            get;
        } = new();

        /// <summary>
        /// Gets the virtual resolution
        /// </summary>
        public BackBuffer BackBuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the renderer
        /// </summary>
        public Renderer Renderer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of random
        /// </summary>
        public Random Random
        {
            get;
        } = new();

        protected override void Initialize()
        {
            BackBuffer = new BackBuffer(GraphicsDevice, ResolutionWidth, ResolutionHeight, ResolutionScaleMode);
            Renderer = new Renderer(GraphicsDevice, BackBuffer);
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            BackBuffer.Dispose();
            Renderer.Dispose();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            BackBuffer.Update();
            Input.Update();
            Coroutines.Update();
            Screens.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(BackBuffer.VirtualBackBuffer);
            Renderer.SetViewport(BackBuffer.FullViewport);
            Renderer.Clear();
            Screens.Draw(Renderer, gameTime);
            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.VirtualBackBufferScaleMatrix);
            Renderer.Draw(BackBuffer.VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }
    }
}
