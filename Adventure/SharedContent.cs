using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    internal static class SharedContent
    {
        public static class Fonts 
        {
            public static SpriteFont Default;
        }

        public static class Graphics
        {
            public static Texture2D Player;
            public static Texture2D Fire;
        }

        public static class Sounds
        {
            public static SoundEffect Shoot;
        }
    }
}
