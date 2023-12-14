using Engine;
using Engine.Extensions;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    internal class LevelTransitionState : LevelLoadState
    {
        private const float ArtificialDelayMs = 3f;
        private const float EllipsisMs = 0.3f; 

        private SpriteFont _spriteFont;
        private float _artificalDelayTimer;
        private float _ellipsisTimer;
        private int _ellipsisCount;
        private string _ellipsis;

        public LevelTransitionState(App application, Level level) : base(application, level)
        {
        }

        public override void Start()
        {
            _spriteFont = Application.Content.Load<SpriteFont>("Fonts/Font");
            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _artificalDelayTimer += gameTime.GetDeltaTime();
            _ellipsisTimer += gameTime.GetDeltaTime();
            if (_ellipsisTimer >= EllipsisMs) 
            {
                _ellipsisTimer = 0;
                _ellipsisCount = (_ellipsisCount + 1) % 4;
                _ellipsis = new string('.', _ellipsisCount);
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw(Application.Resolution.RendererScaleMatrix);
            renderer.DrawString(_spriteFont, "Loading" + _ellipsis, new Vector2(100, 100), Color.White);
            renderer.EndDraw();
        }

        protected override bool CanStartNextLevel()
        {
            return _artificalDelayTimer >= ArtificialDelayMs;
        }
    }
}
