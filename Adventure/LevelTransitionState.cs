using Engine;
using Engine.Extensions;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    internal class LevelTransitionState : LevelLoadState
    {
        private readonly float _artificalDelayMs = 3f;
        private SpriteFont _spriteFont;
        private float _artificalDelayTimer;

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
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw(Application.Resolution.RendererScaleMatrix);
            renderer.DrawString(_spriteFont, "Loading...", new Vector2(100, 100), Color.White);
            renderer.EndDraw();
        }

        protected override bool CanStartNextLevel()
        {
            return _artificalDelayTimer >= _artificalDelayMs;
        }
    }
}
