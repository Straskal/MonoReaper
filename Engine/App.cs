using System;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
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
        public GameStateStack Stack
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
        public VirtualResolution Resolution
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
            Resolution = new VirtualResolution(GraphicsDevice, ResolutionWidth, ResolutionHeight, ResolutionScaleMode);
            Renderer = new Renderer(GraphicsDevice);
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            Resolution.Dispose();
            Renderer.Dispose();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            Resolution.Update();
            Input.Update();
            Coroutines.Update();
            Stack.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(Resolution.RenderTarget);
            Renderer.SetViewport(Resolution.FullViewport);
            Renderer.Clear();
            Stack.Draw(Renderer, gameTime);
            Renderer.SetTarget(null);
            Renderer.SetViewport(Resolution.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(Resolution.ViewportScaleMatrix);
            Renderer.Draw(Resolution.RenderTarget, Vector2.Zero);
            Renderer.EndDraw();
        }
    }
}
