using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    internal sealed class Adventure : App
    {
        private readonly PauseScreen pauseScreen = new();

        public Adventure() : base(256, 256, ResolutionScaleMode.Viewport)
        {
            Instance = this;
            Window.Title = "Adventure Game 2000";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        public static Adventure Instance { get; private set; }
        public static bool IsPaused { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            LoadLevel("Levels/world/level_0");
            LoadLevel("Levels/world/level_1");
            LoadLevel("Levels/world/level_2");
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            LoadSharedContent();
            LoadGUI();
        }

        public void LoadMap(string path) 
        {
            World.Clear();
            World.Spawn(LevelLoader.LoadEntities(this, path));
        }

        public void LoadLevel(string path)
        {
            World.Spawn(LevelLoader.LoadEntities(this, path));
        }

        protected override void Update(GameTime gameTime)
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

            base.Update(gameTime);
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

        private void LoadSharedContent()
        {
            SharedContent.Fonts.Default = Content.Load<SpriteFont>("Fonts/Font");
            SharedContent.Graphics.Player = Content.Load<Texture2D>("Art/Player/Player");
            SharedContent.Graphics.Fire = Content.Load<Texture2D>("Art/Player/Fire");
            SharedContent.Sounds.Shoot = Content.Load<SoundEffect>("Audio/fireball_shoot");
        }

        private void LoadGUI()
        {
            GUI.Renderer = Renderer;
            GUI.BackBuffer = BackBuffer;
        }
    }
}
