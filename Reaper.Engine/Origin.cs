using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;

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

    public static class OriginHelpers 
    {
        public static RectangleF GetOffsetRect(Origin origin, float x, float y, float width, float height) 
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

        public static Vector2 GetOffsetVector(Origin origin, float x, float y, float width, float height)
        {
            var result = GetOffsetRect(origin, x, y, width, height);

            return new Vector2(result.X, result.Y);
        }
    }
}
