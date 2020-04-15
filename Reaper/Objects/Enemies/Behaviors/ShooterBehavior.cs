using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;

namespace Reaper
{
    public class ShooterBehavior : Behavior, IDamageable
    {
        private const float ATTACK_TIME_BUFFER = 2f;

        private float _timer;
        private WorldObject _player;

        private Effect _effect;
        private SoundEffect _hitSound;

        public ShooterBehavior(WorldObject owner) : base(owner)
        {
        }

        public int Health { get; private set; } = 3;

        public override void Load(ContentManager contentManager)
        {
            _effect = contentManager.Load<Effect>("Shaders/SolidColor");
            _hitSound = contentManager.Load<SoundEffect>("audio/hit_hurt");
        }

        public void Damage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
                Owner.Destroy();

            _hitSound.Play();
            Owner.GetBehavior<SpriteSheetBehavior>().Effect = _effect;
            Owner.StartTimer("damaged", 0.1f, () => Owner.GetBehavior<SpriteSheetBehavior>().Effect = null);
        }

        public override void OnLayoutStarted()
        {
            _player = Layout.Objects.FindFirstWithTag("player");
        }

        public override void Tick(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > _timer)
            {
                var direction = Owner.Position - _player.Position;
                direction.Normalize();

                var proj = Layout.Spawn(Projectile.Definition(), Owner.GetPoint("projectileSpawn"));
                proj.ZOrder = Owner.ZOrder + 1;
                proj.GetBehavior<ProjectileBehavior>().Direction = -direction;
                proj.GetBehavior<ProjectileBehavior>().Speed = 150f;
                proj.GetBehavior<ProjectileBehavior>().IgnoreTags = new[] { "enemy" };

                _timer = (float)gameTime.TotalGameTime.TotalSeconds + ATTACK_TIME_BUFFER;
            }
        }
    }
}
