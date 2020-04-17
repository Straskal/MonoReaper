using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;

namespace Reaper
{
    public class ShooterBehavior : Behavior
    {
        private const float ATTACK_TIME_BUFFER = 2f;

        private float _timer;
        private WorldObject _player;
        private WorldObjectPoint _projectileSpawnPoint;

        private SoundEffect _hitSound;

        public ShooterBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 3;

        public override void Load(ContentManager contentManager)
        {
            _hitSound = contentManager.Load<SoundEffect>("audio/hit_hurt");
        }

        public override void OnOwnerCreated()
        {
            Owner.Behaviors.Get<DamageableBehavior>().OnDamaged += OnDamaged;
            _projectileSpawnPoint = Owner.Points.Get("projectileSpawn");
        }

        public override void OnLayoutStarted()
        {
            _player = Layout.Objects.FindFirstWithTag("player");
        }

        public override void Tick(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > _timer)
            {
                var direction = _player.Position - Owner.Position;
                direction.Normalize();

                var proj = Layout.Objects.Create(Projectile.Definition(), _projectileSpawnPoint.Value);
                proj.ZOrder = Owner.ZOrder + 1;
                proj.Behaviors.Get<ProjectileBehavior>().Direction = direction;
                proj.Behaviors.Get<ProjectileBehavior>().Speed = 150f;
                proj.Behaviors.Get<ProjectileBehavior>().IgnoreTags = new[] { "enemy" };

                _timer = (float)gameTime.TotalGameTime.TotalSeconds + ATTACK_TIME_BUFFER;
            }
        }

        public void OnDamaged(DamageableBehavior.DamageInfo info)
        {
            Health -= info.Amount;
            if (Health <= 0)
                Owner.Destroy();

            _hitSound.Play();
        }
    }
}
