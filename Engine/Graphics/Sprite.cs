﻿using System;
using Microsoft.Xna.Framework;
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
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            SourceRectangle = sourceRectangle;
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

        public override void OnDraw(Renderer renderer, GameTime gameTime)
        {
            var origin = Entity.Origin.Tranform(
                Entity.Position.X,
                Entity.Position.Y,
                SourceRectangle.Width,
                SourceRectangle.Height);

            renderer.Draw(_texture, origin.Position, SourceRectangle, Color, SpriteEffects, Effect);
        }
    }
}
