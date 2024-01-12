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
            LoadMap("Levels/world/level_0");
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            LoadSharedContent();
            LoadGUI();
        }

        public void LoadMap(string path) 
        {
            Entities.Clear();
            Entities.Spawn(LevelLoader.LoadLevel(this, path));
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

        protected override void UpdateEntities(GameTime gameTime)
        {
            if (!IsPaused) 
            {
                base.UpdateEntities(gameTime);
            } 
        }

        protected override void DrawEntities(GameTime gameTime)
        {
            base.DrawEntities(gameTime);

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
