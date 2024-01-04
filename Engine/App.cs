﻿using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class App : Game
    {
        private readonly CoroutineRunner _coroutines = new();

        private Screen _nextScreen;

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
        public bool IsDebugDrawEnabled { get; set; }

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
            _coroutines.Update();
            UpdateScreen(gameTime);
            HandleScreenChange();
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(BackBuffer.VirtualBackBuffer);
            Renderer.SetViewport(BackBuffer.FullViewport);
            Renderer.Clear();
            DrawScreen(gameTime);
            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.VirtualBackBufferScaleMatrix);
            Renderer.Draw(BackBuffer.VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }

        protected virtual void UpdateScreen(GameTime gameTime) 
        {
            Screen?.Update(gameTime);
        }

        protected virtual void DrawScreen(GameTime gameTime)
        {
            Screen?.Draw(Renderer, gameTime);
        }

        public void ChangeScreen(Screen screen) 
        {
            _nextScreen = screen;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _coroutines.Start(routine);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            _coroutines.Stop(coroutine);
        }

        private void HandleScreenChange() 
        {
            if (_nextScreen != null) 
            {
                Screen?.Stop();
                Screen = _nextScreen;
                Screen?.Start();
                _nextScreen = null;
            }
        }
    }
}
