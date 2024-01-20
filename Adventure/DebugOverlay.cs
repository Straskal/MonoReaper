using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Adventure
{
    public static class DebugOverlay
    {
        private static bool ShowPlayerMouseLine = false;

        public static void Draw(Renderer renderer)
        {
            HandleInput();
            var mouseWorldPosition = Adventure.Instance.Camera.ToWorld(Input.MousePosition);
            mouseWorldPosition.Round();
            DrawHoveredEntityInfo(renderer, mouseWorldPosition);
            DrawPlayerMouseDirection(renderer, mouseWorldPosition);
        }

        private static void HandleInput()
        {
            if (Input.IsKeyPressed(Keys.D1))
            {
                ShowPlayerMouseLine = !ShowPlayerMouseLine;
            }
        }

        private static void DrawHoveredEntityInfo(Renderer renderer, Vector2 mouseWorldPosition) 
        {
            foreach (var collider in Adventure.Instance.World.OverlapPoint(mouseWorldPosition)) 
            {
                var positionString = collider.Bounds.TopLeft.ToString();
                var positionStringSize = Store.Fonts.Default.MeasureString(positionString);
                var positionStringRect = new RectangleF(collider.Bounds.X, collider.Bounds.Y, positionStringSize.X, positionStringSize.Y);

                positionStringRect.X = MathF.Max(positionStringRect.X, Adventure.Instance.Camera.Bounds.X);
                positionStringRect.Y = MathF.Max(positionStringRect.Y, Adventure.Instance.Camera.Bounds.Y);
                positionStringRect.X = MathF.Min(positionStringRect.X, Adventure.Instance.Camera.Bounds.Right - positionStringRect.Width);
                positionStringRect.Y = MathF.Min(positionStringRect.Y, Adventure.Instance.Camera.Bounds.Bottom - positionStringRect.Height);

                renderer.DrawString(Store.Fonts.Default, collider.Bounds.Position.ToString(), positionStringRect.Position, Color.SpringGreen);
                renderer.DrawRectangleOutline(collider.Bounds.ToXnaRect(), Color.SpringGreen);
            }
        }

        private static void DrawPlayerMouseDirection(Renderer renderer, Vector2 mouseWorldPosition) 
        {
            if (Adventure.Player != null && ShowPlayerMouseLine) 
            {
                renderer.DrawLine(Adventure.Player.Position, mouseWorldPosition, new Color(Color.Violet, 0.05f));
            }
        }
    }
}
