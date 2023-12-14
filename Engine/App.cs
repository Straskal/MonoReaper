using System;
using System.Collections;
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

        private readonly CoroutineRunner _coroutineRunner = new();

        public App(int targetResolutionWidth, int targetResolutionHeight, ResolutionScaleMode resolutionScaleMode)
        {
            Instance = this;
            ResolutionWidth = targetResolutionWidth;
            ResolutionHeight = targetResolutionHeight;
            ResolutionScaleMode = resolutionScaleMode;
            Content = new ContentManagerExtended(Services, "Content");
            Stack = new GameStateStack(this);

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
        /// Gets the game state stack
        /// </summary>
        public GameStateStack Stack
        {
            get;
        }

        /// <summary>
        /// Gets the instance of random
        /// </summary>
        public Random Random
        {
            get;
        } = new();

        /// <summary>
        /// Starts and returns a coroutine created with the given enumerator.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator enumerator) 
        {
            return _coroutineRunner.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stops the given coroutine.
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopCoroutine(Coroutine coroutine) 
        {
            _coroutineRunner.StopCoroutine(coroutine);
        }

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
            _coroutineRunner.Update();
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
