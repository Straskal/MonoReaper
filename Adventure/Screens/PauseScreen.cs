using Microsoft.Xna.Framework;
using Engine;
using Engine.Graphics;

namespace Adventure
{
    internal class PauseScreen : Screen
    {
        public PauseScreen(App application) : base(application)
        {
        }

        public override bool ShouldUpdateBelow
        {
            // Don't update the states below. This is what pauses the game.
            get => false;
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw();
            DrawOverlay(renderer);
            GUI.Start();
            if (GUI.PrimaryButton(1, "Resume", 100, 150))
            {
                Application.Screens.Pop(this);
            }
            if (GUI.PrimaryButton(3, "Main menu", 100, 180))
            {
                Application.Screens.SetTop(new MainMenuScreen(Application));
            }
            renderer.EndDraw();
        }

        private void DrawOverlay(Renderer renderer) 
        {
            renderer.DrawRectangle(0, 0, Application.ResolutionWidth, Application.ResolutionHeight, new Color(Color.Black, 0.6f));
            renderer.DrawString(SharedContent.Fonts.Default, "Paused", 100f, 100f, Color.White);
        }
    }
}
