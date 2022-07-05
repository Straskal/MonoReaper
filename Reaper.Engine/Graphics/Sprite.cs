using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public class Sprite : Component
    {
        public Sprite(Texture2D texture) : this(texture, Rectangle.Empty)
        {
        }

        public Sprite(Texture2D texture, Rectangle sourceRectangle)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
            IsTickEnabled = false;
            IsDrawEnabled = true;
        }

        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public bool IsMirrored { get; set; }

        public override void OnDraw()
        {
            var offset = Offset.GetVector(
                Entity.Origin,
                Entity.Position.X,
                Entity.Position.Y,
                SourceRectangle.Width,
                SourceRectangle.Height);

            Renderer.Draw(Texture, SourceRectangle, new Vector2(offset.X, offset.Y), Color.White, IsMirrored, Effect);
        }
    }
}
