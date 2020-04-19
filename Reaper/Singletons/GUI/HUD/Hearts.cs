using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;

namespace Reaper
{
    public class Hearts : Singleton
    {
        private Texture2D _heartImage;
        private PlayerBehavior _player;

        public Hearts(MainGame game) : base(game) { }

        public override void OnLayoutStarted()
        {
            _player = CurrentLayout.Objects.FindFirstWithTag("player")?.Behaviors.Require<PlayerBehavior>();
        }

        public override void Load()
        {
            _heartImage = Game.Content.Load<Texture2D>("art/player/heart");
        }

        public override void DrawGUI(Renderer renderer)
        {
            if (_player != null) 
            {
                Vector2 drawPosition = CurrentLayout.View.ToScreen(new Vector2(10f, 10f));

                for (int i = 0; i < _player.Health; i++) 
                {
                    renderer.Draw(_heartImage, drawPosition, Color.White);
                    drawPosition.X += _heartImage.Width + 2;
                }
            }
        }
    }
}
