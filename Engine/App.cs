using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class App : Game
    {
        private readonly CoroutineRunner coroutines = new();

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
        public Camera Camera { get; private set; }
        public EntityManager Entities { get; private set; }
        public bool Debug { get; set; }

        protected override void Initialize()
        {
            BackBuffer = new BackBuffer(GraphicsDevice, ResolutionWidth, ResolutionHeight, ResolutionScaleMode);
            Renderer = new Renderer(GraphicsDevice, BackBuffer);
            Camera = new Camera(BackBuffer);
            Entities = new EntityManager();
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
            UpdateEntities(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            BeginDrawFrame();
            DrawEntities(gameTime);
            EndDrawFrame();
        }

        protected virtual void UpdateEntities(GameTime gameTime) 
        {
            Entities.Update(gameTime);
        }

        protected void BeginDrawFrame() 
        {
            Renderer.SetTarget(BackBuffer.VirtualBackBuffer);
            Renderer.SetViewport(BackBuffer.FullViewport);
            Renderer.Clear();
        }

        protected void EndDrawFrame()
        {
            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.VirtualBackBufferScaleMatrix);
            Renderer.Draw(BackBuffer.VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }

        protected virtual void DrawEntities(GameTime gameTime)
        {
            Renderer.BeginDraw(Camera.TransformationMatrix);
            Entities.Draw(Renderer, gameTime);

            if (Debug) 
            {
                Entities.DebugDraw(Renderer, gameTime);
            }

            Renderer.EndDraw();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return coroutines.Start(routine);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            coroutines.Stop(coroutine);
        }
    }
}
