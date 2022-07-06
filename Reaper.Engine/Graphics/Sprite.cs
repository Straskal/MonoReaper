using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public class Sprite : Component
    {
        private Texture2D _texture;

        public Sprite(Texture2D texture) : this(texture, Rectangle.Empty)
        {
        }

        public Sprite(Texture2D texture, Rectangle sourceRectangle)
        {
            _texture = texture;
            SourceRectangle = sourceRectangle;
            IsTickEnabled = false;
            IsDrawEnabled = true;
        }

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
            var offset = Offset.GetVector(
                Entity.Origin,
                Entity.Position.X,
                Entity.Position.Y,
                SourceRectangle.Width,
                SourceRectangle.Height);

            Renderer.Draw(_texture, SourceRectangle, new Vector2(offset.X, offset.Y), Color, IsMirrored, Effect);
        }
    }
}
