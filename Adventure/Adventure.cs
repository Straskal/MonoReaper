using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public sealed class Adventure : Game
    {
        public const int RESOLUTION_WIDTH = 256;
        public const int RESOLUTION_HEIGHT = 256;

        private readonly PauseScreen pauseScreen = new();

        public Adventure()
        {
            Instance = this;
            Window.Title = "Adventure Game 2000";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            Content = new ContentManager(Services, "Content");
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = RESOLUTION_WIDTH;
            GraphicsDeviceManager.PreferredBackBufferHeight = RESOLUTION_HEIGHT;
            IsFixedTimeStep = true;
        }

        public static Adventure Instance { get; private set; }
        public static bool IsPaused { get; set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public BackBuffer BackBuffer { get; private set; }
        public Renderer Renderer { get; private set; }
        public Camera Camera { get; private set; }
        public World World { get; private set; }
        public bool Debug { get; set; }

        protected override void Initialize()
        {
            BackBuffer = new BackBuffer(GraphicsDevice, RESOLUTION_WIDTH, RESOLUTION_HEIGHT, ResolutionScaleMode.Viewport);
            Renderer = new Renderer(GraphicsDevice, BackBuffer);
            Camera = new Camera(BackBuffer);
            World = new World();
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
            LoadLevel("Levels/world/level_0");
            LoadLevel("Levels/world/level_1");
            LoadLevel("Levels/world/level_2");
        }

        public void LoadLevel(string path)
        {
            var data = Content.Load<LevelData>(path);
            World.Spawn(Level.GetEntities(data));
            World.Spawn(new CameraArea(new RectangleF(data.Bounds)));
        }

        protected override void Update(GameTime gameTime)
        {
            BackBuffer.Update();
            Input.Update(BackBuffer);
            if (!IsPaused)
            {
                World.Update(gameTime);
            }
            HandleGlobalInput();
            ScreenShake.Update(gameTime);
            RoundCameraPosition();
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.SetTarget(BackBuffer.VirtualBackBuffer);
            Renderer.SetViewport(BackBuffer.FullViewport);
            Renderer.Clear();
            Renderer.BeginDraw(Camera.TransformationMatrix);
            World.Draw(Renderer, gameTime);
            if (Debug)
            {
                World.DebugDraw(Renderer, gameTime);
            }
            Renderer.EndDraw();
            if (IsPaused)
            {
                pauseScreen.Draw(Renderer, gameTime);
            }
            Renderer.SetTarget(null);
            Renderer.SetViewport(BackBuffer.LetterboxViewport);
            Renderer.Clear();
            Renderer.BeginDraw(BackBuffer.VirtualBackBufferScaleMatrix);
            Renderer.Draw(BackBuffer.VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }

        private void LoadAllContent()
        {
            Store.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
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

            if (Input.IsKeyPressed(Keys.Space))
            {
                Camera.Zoom = Camera.Zoom - 0.25f;

                if (Camera.Zoom <= 0f)
                {
                    Camera.Zoom = 2f;
                }
            }
        }

        private void RoundCameraPosition()
        {

        }
    }
}
