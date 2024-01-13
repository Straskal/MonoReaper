using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    internal static class Store
    {
        public static class Fonts 
        {
            public static SpriteFont Default;
        }

        public static class Gfx
        {
            public static Texture2D Player;
            public static Texture2D Barrel;
            public static Texture2D Fire;
            public static Texture2D Explosion;
        }
        public static class Vfx
        {
            public static Effect SolidColor;
        }

        public static class Sfx
        {
            public static SoundEffect Shoot;
            public static SoundEffect Explosion;
        }
    }
}
