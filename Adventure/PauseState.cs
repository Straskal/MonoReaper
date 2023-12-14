using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Graphics;

namespace Adventure
{
    internal class PauseState : GameState
    {
        public override bool ShouldUpdateBelow
        {
            // Don't update the states below. This is what pauses the game.
            get => false;
        }

        public override bool ShouldDrawBelow
        {
            // Draw the states below
            get => true;
        }

        public SpriteFont SpriteFont
        {
            get;
            private set;
        }

        public override void Start()
        {
            SpriteFont = Application.Content.Load<SpriteFont>("Fonts/Font");
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw(Application.VirtualResolution.ScaleTransformationMatrix);
            renderer.DrawRectangle(new Rectangle(0, 0, Application.ResolutionWidth, Application.ResolutionHeight), new Color(Color.Black, 0.4f));
            renderer.DrawString(SpriteFont, "Paused", new Vector2(100, 100), Color.White);
            renderer.EndDraw();
        }
    }
}
