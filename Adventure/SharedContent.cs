using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    internal static class SharedContent
    {
        public static SpriteFont Font { get; set; }

        public static class Gfx
        {
            public static Texture2D Player;
            public static Texture2D Fire;
        }

        public static class Sfx
        {
            public static SoundEffect Shoot;
        }
    }
}
