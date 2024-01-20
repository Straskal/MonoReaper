using Engine;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    internal class DialogOverlay
    {
        private bool hasDialog;

        public void Draw(Renderer renderer)
        {
            HandleInput();

            DrawTextBox(renderer, "Hello World!");
        }

        private void HandleInput()
        {
            if (Input.IsKeyPressed(Keys.T))
            {
                hasDialog = !hasDialog;
            }
        }

        private void DrawTextBox(Renderer renderer, string text)
        {
            if (hasDialog)
            {
                renderer.BeginDraw();

                GUI.DialogBox(text, 0, 175);

                renderer.EndDraw();
            }
        }
    }
}
