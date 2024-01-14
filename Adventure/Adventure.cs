using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public sealed class Adventure : App
    {
        public const int RESOLUTION_WIDTH = 256;
        public const int RESOLUTION_HEIGHT = 256;

        private readonly PauseScreen pauseScreen = new();

        public Adventure() : base(RESOLUTION_WIDTH, RESOLUTION_HEIGHT, ResolutionScaleMode.Viewport)
        {
            Instance = this;
            Window.Title = "Adventure Game 2000";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        public static Adventure Instance { get; private set; }
        public static bool IsPaused { get; set; }

        protected override void LoadContent()
        {
            base.LoadContent();
            LoadAllContent();
            LoadGUI();
            LoadMap();
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
            base.Update(gameTime);
            HandleGlobalInput();
            ScreenShake.Update(gameTime);
            RoundCameraPosition();
        }

        protected override void UpdateFrame(GameTime gameTime)
        {
            if (!IsPaused) 
            {
                base.UpdateFrame(gameTime);
            } 
        }

        protected override void DrawFrame(GameTime gameTime)
        {
            base.DrawFrame(gameTime);

            if (IsPaused) 
            {
                pauseScreen.Draw(Renderer, gameTime);
            }
        }

        private void LoadAllContent()
        {
            Store.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
            Store.Gfx.Player = Content.Load<Texture2D>("art/player/player");
            Store.Gfx.Fire = Content.Load<Texture2D>("art/player/fire");
            Store.Gfx.Barrel = Content.Load<Texture2D>("art/common/barrel");
            Store.Gfx.Explosion = Content.Load<Texture2D>("art/common/explosion-1");
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

        private void RoundCameraPosition()
        {
            Camera.Position = Vector2.Floor(Camera.Position + new Vector2(0.5f));
        }
    }
}
