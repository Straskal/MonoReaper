using Microsoft.Xna.Framework;
using Reaper.Engine.Collision;

namespace Reaper.Engine
{
    public enum Origin
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public static class Offset 
    {
        public static RectangleF GetRect(Origin origin, float x, float y, float width, float height) 
        {
            RectangleF rect;
            rect.Width = width;
            rect.Height = height;

            switch (origin)
            {
                case Origin.TopLeft:
                default:
                    rect.X = x;
                    rect.Y = y;
                    break;
                case Origin.TopCenter:
                    rect.X = x - rect.Width / 2;
                    rect.Y = y;
                    break;
                case Origin.TopRight:
                    rect.X = x - rect.Width;
                    rect.Y = y;
                    break;
                case Origin.CenterLeft:
                    rect.X = x;
                    rect.Y = y - rect.Height / 2;
                    break;
                case Origin.Center:
                    rect.X = x - rect.Width / 2;
                    rect.Y = y - rect.Height / 2;
                    break;
                case Origin.CenterRight:
                    rect.X = x - rect.Width;
                    rect.Y = y - rect.Height / 2;
                    break;
                case Origin.BottomLeft:
                    rect.X = x;
                    rect.Y = y - rect.Height;
                    break;
                case Origin.BottomCenter:
                    rect.X = x - rect.Width / 2;
                    rect.Y = y - rect.Height;
                    break;
                case Origin.BottomRight:
                    rect.X = x - rect.Width;
                    rect.Y = y - rect.Height;
                    break;
            }

            return rect;
        }

        public static Vector2 GetVector(Origin origin, float x, float y, float width, float height)
        {
            var result = GetRect(origin, x, y, width, height);

            return new Vector2(result.X, result.Y);
        }

        public static Vector2 Create(Origin origin, float x, float y, float width, float height) 
        {
            Vector2 position;

            switch (origin)
            {
                case Origin.TopLeft:
                default:
                    position.X = x;
                    position.Y = y;
                    break;
                case Origin.TopCenter:
                    position.X = x + width / 2;
                    position.Y = y;
                    break;
                case Origin.TopRight:
                    position.X = x + width;
                    position.Y = y;
                    break;
                case Origin.CenterLeft:
                    position.X = x;
                    position.Y = y + height / 2;
                    break;
                case Origin.Center:
                    position.X = x + width / 2;
                    position.Y = y + height / 2;
                    break;
                case Origin.CenterRight:
                    position.X = x + width;
                    position.Y = y + height / 2;
                    break;
                case Origin.BottomLeft:
                    position.X = x;
                    position.Y = y + height;
                    break;
                case Origin.BottomCenter:
                    position.X = x + width / 2;
                    position.Y = y + height;
                    break;
                case Origin.BottomRight:
                    position.X = x + width;
                    position.Y = y + height;
                    break;
            }

            return position;
        }
    }
}
