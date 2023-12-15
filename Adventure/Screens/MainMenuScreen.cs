using Microsoft.Xna.Framework;
using Engine;
using Engine.Graphics;

namespace Adventure
{
    internal class MainMenuScreen : Screen
    {
        public MainMenuScreen(App application) : base(application)
        {
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw();
            DrawGameTitle(renderer);
            DrawButtons();
            renderer.EndDraw();
        }

        private static void DrawGameTitle(Renderer renderer)
        {
            renderer.DrawString(SharedContent.Fonts.Default, "Super great game 2000", new Vector2(50f, 10f), Color.White);
        }

        private void DrawButtons() 
        {
            GUI.Start();

            if (GUI.PrimaryButton(1, "Start", 100, 120)) 
            {
                Screens.SetTop(new LevelTransitionScreen(Application, LevelLoader.LoadLevel(Application, "Levels/world/level_0")));
            }

            if (GUI.DangerButton(2, "Exit", 100, 150))
            {
                Application.Exit();
            }
        }
    }
}
