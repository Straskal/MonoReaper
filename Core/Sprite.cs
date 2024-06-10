using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Sprite
    {
        public Sprite(Texture2D texture)
        {
            Texture = texture;
            SourceRectangle = Texture.Bounds;
        }

        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects SpriteEffects { get; set; }
    }
}
