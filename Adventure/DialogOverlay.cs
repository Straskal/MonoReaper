using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Adventure
{
    internal class DialogOverlay
    {
        private bool hasDialog;
        public string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            return source.Remove(place, find.Length).Insert(place, replace);
        }

        public void Draw(Renderer renderer)
        {
            HandleInput();

            var text = WrapText(Store.Fonts.Default, "1WWWW 2WWWW 3WWWW 4WWWW 5WWWW 6WWWW 7WWWW 8WWWW 9fWWWW", 225);

            if (hasDialog)
            {
                renderer.BeginDraw();

                var x = 0;
                var y = 200;

                renderer.DrawRectangle(new Rectangle(x + 7, y, 245, 75), Color.DarkRed);
                renderer.DrawRectangle(new Rectangle(x + 10, y + 3, 38, 50), Color.Black);

                renderer.DrawString(Store.Fonts.Default, text, new Vector2(x + 20, y), Color.White);

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

        // Stackoverflow: https://stackoverflow.com/questions/15986473/how-do-i-implement-word-wrap
        private string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
