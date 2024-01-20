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
            if (GUI.PrimaryButton(3, "Restart", 100, 180))
            {
                Adventure.IsPaused = false;
                Adventure.Instance.LoadMap();
            }
            renderer.EndDraw();
        }

        private void DrawOverlay(Renderer renderer)
        {
            renderer.DrawRectangle(0, 0, Adventure.RESOLUTION_WIDTH, Adventure.RESOLUTION_HEIGHT, new Color(Color.Black, 0.6f));
            renderer.DrawString(Store.Fonts.Default, "Paused", 100f, 100f, Color.White);
        }
    }
}
