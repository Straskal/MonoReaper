using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace Adventure
{
    /// <summary>
    /// The root state acts as the entry point to the application and overall game controller.
    /// The root state cannot be popped from the stack.
    /// </summary>
    internal class RootScreen : Screen
    {
        private readonly PauseScreen _pauseScreen;

        public RootScreen(App application) : base(application)
        {
            _pauseScreen = new PauseScreen(application);
        }

        public override void Start()
        {
            Application.Window.Title = "Adventure Game 2000";
            Application.Window.AllowUserResizing = true;
            Application.IsMouseVisible = true;

            LoadSharedContent();
            LoadGUI();

            Screens.Push(LevelLoader.LoadLevel(Application, "Levels/world/level_0"));
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.IsKeyPressed(Keys.F))
            {
                Application.GraphicsDeviceManager.ToggleFullScreen();
                Application.GraphicsDeviceManager.ApplyChanges();
            }

            if (Input.IsKeyPressed(Keys.Escape))
            {
                if (Screens.Top == _pauseScreen)
                {
                    Screens.Pop(_pauseScreen);
                }
                else if (Screens.Top is Level)
                {
                    Screens.Push(_pauseScreen);
                }
            }
        }

        private void LoadSharedContent()
        {
            SharedContent.Fonts.Default = Application.Content.Load<SpriteFont>("Fonts/Font");
            SharedContent.Graphics.Player = Application.Content.Load<Texture2D>("Art/Player/Player");
            SharedContent.Graphics.Fire = Application.Content.Load<Texture2D>("Art/Player/Fire");
            SharedContent.Sounds.Shoot = Application.Content.Load<SoundEffect>("Audio/fireball_shoot");
        }

        private void LoadGUI() 
        {
            GUI.Renderer = Application.Renderer;
            GUI.BackBuffer = Application.BackBuffer;
        }
    }
}
