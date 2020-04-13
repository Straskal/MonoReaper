using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper;
using Reaper.Engine;

namespace Reaper
{
    public class BlobBehavior : Behavior, IDamageable
    {
        private Effect _effect;

        public BlobBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 3;
        public float Speed { get; set; } = 50f;
        public Vector2 Direction { get; set; } = new Vector2(0f, 1f);

        public override void Load(ContentManager contentManager)
        {
            _effect = contentManager.Load<Effect>("Shaders/SolidColor");
        }

        public void Damage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
                Owner.Destroy();

            Owner.GetBehavior<SpriteSheetBehavior>().Effect = _effect;
            Owner.StartTimer("damaged", 0.1f, () => Owner.GetBehavior<SpriteSheetBehavior>().Effect = null);
        }

        public override void Tick(GameTime gameTime)
        {
            if (Owner.MoveAndCollide(Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, out var _))
            {
                Direction *= new Vector2(0, -1f);
            }    
        }
    }
}
