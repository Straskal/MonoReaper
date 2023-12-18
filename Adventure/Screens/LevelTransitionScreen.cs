using Engine;
using Engine.Extensions;
using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Adventure
{
    internal class LevelTransitionScreen : LevelLoadingScreen
    {
        private const float ArtificialDelayTime = 2f;
        private const float EllipsisTime = 0.3f; 

        private float _artificalDelayTimer;
        private float _ellipsisTimer;
        private int _ellipsisCount;
        private string _ellipsis;

        public LevelTransitionScreen(App application, Level level) : base(application, level)
        {
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDelayTimer(gameTime);
            UpdateLoadingEllipsis(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            DrawLoadingAnimation(renderer);
            base.Draw(renderer, gameTime);
        }

        //protected override bool CanStartNextLevel()
        //{
        //    return _artificalDelayTimer >= ArtificialDelayTime;
        //}

        private void UpdateDelayTimer(GameTime gameTime) 
        {
            _artificalDelayTimer += gameTime.GetDeltaTime();
        }

        private void UpdateLoadingEllipsis(GameTime gameTime) 
        {
            if ((_ellipsisTimer += gameTime.GetDeltaTime()) >= EllipsisTime)
            {
                _ellipsisTimer = 0;
                _ellipsisCount = (_ellipsisCount + 1) % 4;
                _ellipsis = new string('.', _ellipsisCount);
            }
        }

        private void DrawLoadingAnimation(Renderer renderer) 
        {
            renderer.BeginDraw();
            renderer.DrawString(SharedContent.Fonts.Default, "Loading" + _ellipsis, new Vector2(100, 100), Color.White);
            renderer.EndDraw();
        }
    }
}
