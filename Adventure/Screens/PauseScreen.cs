using Microsoft.Xna.Framework;
using Engine;

namespace Adventure
{
    internal class PauseScreen
    {
        public void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw();
            DrawOverlay(renderer);
            GUI.Start();
            if (GUI.PrimaryButton(1, "Resume", 100, 150))
            {
                Adventure.IsPaused = false;
            }
            if (GUI.PrimaryButton(3, "Main menu", 100, 180))
            {
                Adventure.IsPaused = false;
                Adventure.Instance.ChangeScreen(new MainMenuScreen(Adventure.Instance));
            }
            renderer.EndDraw();
        }

        private void DrawOverlay(Renderer renderer) 
        {
            renderer.DrawRectangle(0, 0, Adventure.Instance.ResolutionWidth, Adventure.Instance.ResolutionHeight, new Color(Color.Black, 0.6f));
            renderer.DrawString(SharedContent.Fonts.Default, "Paused", 100f, 100f, Color.White);
        }
    }
}
