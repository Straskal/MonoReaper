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
    internal class RootState : GameState
    {
        private readonly PauseState _pauseState;

        public RootState(App application) : base(application)
        {
            _pauseState = new PauseState(application);
        }

        public override void Start()
        {
            Application.Window.Title = "Adventure Game 2000";
            Application.Window.AllowUserResizing = true;

            LoadSharedContent();
            LoadGUI();

            Stack.Push(new MainMenuState(Application));
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
                if (Stack.Top == _pauseState)
                {
                    Stack.Pop(_pauseState);
                }
                else
                {
                    Stack.Push(_pauseState);
                }
            }
        }

        private void LoadSharedContent()
        {
            SharedContent.Font = Application.Content.Load<SpriteFont>("Fonts/Font");
            SharedContent.Gfx.Player = Application.Content.Load<Texture2D>("art/player/player");
            SharedContent.Gfx.Fire = Application.Content.Load<Texture2D>("art/player/fire");
            SharedContent.Sfx.Shoot = Application.Content.Load<SoundEffect>("audio/fireball_shoot");
        }

        private void LoadGUI() 
        {
            GUI.Renderer = Application.Renderer;
            GUI.Screen = Application.Screen;
        }
    }
}
