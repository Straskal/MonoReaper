using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine.Graphics
{
    public class Sprite : Component
    {
        private readonly string _texturePath;

        public Sprite(string texturePath)
        {
            _texturePath = texturePath;

            IsTickEnabled = false;
            IsDrawEnabled = true;
        }

        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public bool IsMirrored { get; set; }

        public override void OnLoad(ContentManager content)
        {
            Texture = content.Load<Texture2D>(_texturePath);
        }

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
