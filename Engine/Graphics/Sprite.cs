using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    /// <summary>
    /// A sprite represents a texture and a texture source drawn for an entity.
    /// </summary>
    public class Sprite : Component
    {
        public Sprite(Texture2D texture) : this(texture, Rectangle.Empty)
        {
        }

        public Sprite(Texture2D texture, Rectangle sourceRectangle)
        {
            ArgumentNullException.ThrowIfNull(texture, nameof(texture));

            Texture = texture;
            SourceRectangle = sourceRectangle;
            IsDrawEnabled = true;
        }

        private Texture2D _texture;

        /// <summary>
        /// Gets or sets the sprite's texture
        /// </summary>
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

        /// <summary>
        /// Gets or sets the sprite's effect
        /// </summary>
        public Effect Effect 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the sprites rectangle
        /// </summary>
        public Rectangle SourceRectangle 
        {
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the sprites color
        /// </summary>
        public Color Color 
        { 
            get; 
            set; 
        } = Color.White;

        /// <summary>
        /// Gets or sets the sprite's sprite effects.
        /// </summary>
        public SpriteEffects SpriteEffects 
        {
            get;
            set;
        }

        public override void OnLoad(ContentManager content)
        {
            _texture ??= Renderer.BlankTexture;
        }

        public override void OnDraw()
        {
            var offsetPosition = Offset.GetVector(
                Entity.Origin,
                Entity.Position.X,
                Entity.Position.Y,
                SourceRectangle.Width,
                SourceRectangle.Height);

            Renderer.Draw(_texture, offsetPosition, SourceRectangle, Color, SpriteEffects, Effect);
        }
    }
}
