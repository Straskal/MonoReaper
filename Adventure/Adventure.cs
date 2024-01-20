using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Adventure
{
    public sealed class Adventure : Game
    {
        public const int RESOLUTION_WIDTH = 256;
        public const int RESOLUTION_HEIGHT = 256;
        public const int WORLD_CELL_SIZE = 128;

        private readonly PauseScreen pauseScreen = new();
        private readonly List<CameraZone> cameraZones = new();

        public Adventure()
        {
            Instance = this;
            Window.Title = "Adventure Game 2000";
            Window.AllowUserResizing = true;
            IsMouseVisible = false;
            Content = new ContentManager(Services, "Content");
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = RESOLUTION_WIDTH;
            GraphicsDeviceManager.PreferredBackBufferHeight = RESOLUTION_HEIGHT;
            IsFixedTimeStep = true;
        }

        public static Adventure Instance { get; private set; }
        public static GameTime Time { get; private set; }
        public static Player Player { get; private set; }
        public static bool IsPaused { get; set; }
        public static bool IsTransitioningAreas { get; set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public BackBuffer BackBuffer { get; private set; }
        public Renderer Renderer { get; private set; }
        public Camera Camera { get; private set; }
        public World World { get; private set; }
        public CoroutineRunner Coroutines { get; } = new();
        public bool Debug { get; set; }

        protected override void Initialize()
        {
            BackBuffer = new BackBuffer(Window, GraphicsDevice, RESOLUTION_WIDTH, RESOLUTION_HEIGHT);
            Renderer = new Renderer(GraphicsDevice);
            Camera = new Camera(RESOLUTION_WIDTH, RESOLUTION_HEIGHT);
            World = new World(WORLD_CELL_SIZE);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadAllContent();
            LoadGUI();
            LoadMap();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            BackBuffer.Dispose();
            Renderer.Dispose();
            Content.Unload();
            base.UnloadContent();
        }

        public void LoadMap()
        {
            World.Clear();
            cameraZones.Clear();
            Player = null;

            LoadLevel("Levels/world/level_0");
            LoadLevel("Levels/world/level_1");
            LoadLevel("Levels/world/level_2");

            Player = World.Find<Player>();
        }

        public void LoadLevel(string path)
        {
            var data = Content.Load<LevelData>(path);
            cameraZones.Add(new CameraZone(new RectangleF(data.Bounds)));
            World.Spawn(Level.GetEntities(data));
        }

        protected override void Update(GameTime gameTime)
        {
            Time = gameTime;
            HandleGlobalInput();
            Input.Update(BackBuffer);
            Coroutines.Update();

            if (!IsPaused && !IsTransitioningAreas)
            {
                World.Update(gameTime);
            }

            foreach (var cameraZone in cameraZones)
            {
                cameraZone.CheckForPlayer();
            }

            ScreenShake.Update(gameTime);

            Camera.Position.Round();
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(BackBuffer.RenderTarget);
            Renderer.SetViewport(BackBuffer.RenderTargetViewport);
            Renderer.Clear();
            Renderer.BeginDraw(Camera.TransformationMatrix);
            World.Draw(Renderer, gameTime);
            Renderer.EndDraw();

            if (Debug)
            {
                Renderer.BeginDraw(Camera.TransformationMatrix);
                World.DebugDraw(Renderer, gameTime);
                DebugOverlay.Draw(Renderer);
                Renderer.EndDraw();
            }


            if (IsPaused)
            {
                pauseScreen.Draw(Renderer, gameTime);
            }

            DrawCursor();

            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.ScaleMatrix);
            Renderer.Draw(BackBuffer.RenderTarget, Vector2.Zero);
            Renderer.EndDraw();
        }

        private void DrawCursor()
        {
            var source = Input.IsMouseLeftDown()
                ? new Rectangle(0, 0, 8, 8)
                : new Rectangle(8, 0, 8, 8);

            var cursorOffset = source.Size.ToVector2() / 2f;
            var cursorPosition = Input.MousePosition - cursorOffset;

            Renderer.BeginDraw();
            Renderer.Draw(Store.Gfx.Cursor, cursorPosition, source);
            Renderer.EndDraw();
        }

        private void LoadAllContent()
        {
            Store.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
            Store.Gfx.Cursor = Content.Load<Texture2D>("art/cursor");
            Store.Gfx.Player = Content.Load<Texture2D>("art/player/player");
            Store.Gfx.Fire = Content.Load<Texture2D>("art/player/fire");
            Store.Gfx.Barrel = Content.Load<Texture2D>("art/common/barrel");
            Store.Gfx.Explosion = Content.Load<Texture2D>("art/common/explosion-1");
            Store.Gfx.LargeDoor = Content.Load<Texture2D>("art/large_door");
            Store.Gfx.PressurePlate = Content.Load<Texture2D>("art/pressure_plate");
            Store.Vfx.SolidColor = Content.Load<Effect>("shaders/SolidColor");
            Store.Sfx.Shoot = Content.Load<SoundEffect>("audio/fireball_shoot");
            Store.Sfx.Explosion = Content.Load<SoundEffect>("audio/explosion4");
        }

        private void LoadGUI()
        {
            GUI.Renderer = Renderer;
            GUI.BackBuffer = BackBuffer;
        }

        private void HandleGlobalInput()
        {
            if (Input.IsKeyPressed(Keys.F))
            {
                GraphicsDeviceManager.ToggleFullScreen();
                GraphicsDeviceManager.ApplyChanges();
            }

            if (Input.IsKeyPressed(Keys.Escape))
            {
                IsPaused = !IsPaused;
            }

            if (Input.IsKeyPressed(Keys.OemTilde))
            {
                Debug = !Debug;
            }
        }
    }
}
