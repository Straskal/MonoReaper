﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Sprite : GraphicsComponent
    {
        public Sprite(Entity entity, Texture2D texture) : base(entity)
        {
            Texture = texture;
        }

        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects SpriteEffects { get; set; }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            var bounds = Entity.TransformOrigin(SourceRectangle.Width, SourceRectangle.Height);

            renderer.Draw(Texture, bounds.Position, SourceRectangle, Color, SpriteEffects, Effect);
        }
    }
}
