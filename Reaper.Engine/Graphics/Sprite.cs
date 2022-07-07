using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        private Texture2D _texture;
        public Texture2D Texture
        {
            get => _texture;
            set
            {
                if (value != null)
                {
                    _texture = value;
                }
            }
        }

        public Effect Effect { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Color { get; set; } = Color.White;
        public bool IsMirrored { get; set; }

        public override void OnLoad(ContentManager content)
        {
            if (_texture == null)
            {
                _texture = Renderer.BlankTexture;
            }
        }

        public override void OnDraw()
        {
            var origin = Entity.Origin;
            var position = Entity.Position;
            var sourceRect = SourceRectangle;
            var offsetPosition = Offset.GetVector(
                origin,
                position.X,
                position.Y,
                sourceRect.Width,
                sourceRect.Height);

            Renderer.Draw(_texture, sourceRect, offsetPosition, Color, IsMirrored, Effect);
        }
    }
}
