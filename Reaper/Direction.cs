using Microsoft.Xna.Framework;

namespace Reaper
{
    public static class Direction
    {
        public static Vector2 Up => new Vector2(0f, -1f);
        public static Vector2 Down => new Vector2(0f, 1f);
        public static Vector2 Left => new Vector2(-1f, 0f);
        public static Vector2 Right => new Vector2(1f, 0f);
    }
}
