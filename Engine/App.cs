using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class App : Game
    {
        private readonly CoroutineRunner coroutines = new();

        private Screen nextScreen;

        public App(int resolutionWidth, int resolutionHeight, ResolutionScaleMode resolutionScaleMode)
        {
            ResolutionWidth = resolutionWidth;
            ResolutionHeight = resolutionHeight;
            ResolutionScaleMode = resolutionScaleMode;
            Content = new ContentManagerExtended(Services, "Content");
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
            IsFixedTimeStep = true;
        }

        public static Random Random { get; } = new();

        public int ResolutionWidth { get; }
        public int ResolutionHeight { get; }
        public ResolutionScaleMode ResolutionScaleMode { get; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public BackBuffer BackBuffer { get; private set; }
        public Renderer Renderer { get; private set; }
        public Screen Screen { get; private set; }

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
            Input.Update(BackBuffer);
            coroutines.Update();
            Screen?.Update(gameTime);
            HandleScreenChange();
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(BackBuffer.VirtualBackBuffer);
            Renderer.SetViewport(BackBuffer.FullViewport);
            Renderer.Clear();
            Screen?.Draw(Renderer, gameTime);
            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.VirtualBackBufferScaleMatrix);
            Renderer.Draw(BackBuffer.VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }

        public void ChangeScreen(Screen screen) 
        {
            nextScreen = screen;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return coroutines.Start(routine);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            coroutines.Stop(coroutine);
        }

        private void HandleScreenChange() 
        {
            if (nextScreen != null) 
            {
                Screen?.Stop();
                Screen = nextScreen;
                Screen?.Start();
                nextScreen = null;
            }
        }
    }
}
