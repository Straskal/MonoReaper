using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    internal class DialogOverlay
    {
        private bool hasDialog;

        public void Draw(Renderer renderer)
        {
            HandleInput();

            var text = "Hello World!";

            if (hasDialog)
            {
                renderer.BeginDraw();

                var x = 0;
                var y = 175;

                renderer.DrawRectangle(new Rectangle(x + 5, y, 245, 75), Color.Orange);
                renderer.DrawRectangle(new Rectangle(x + 10, y + 5, 235, 65), Color.Pink);

                renderer.DrawString(Store.Fonts.Default, text, new Vector2(x + 20, y + 5), Color.Black);

                renderer.EndDraw();
            }
        }

        private void HandleInput()
        {
            if (Input.IsKeyPressed(Keys.T))
            {
                hasDialog = !hasDialog;
            }
        }
    }
}
