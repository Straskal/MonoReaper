using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine;

namespace Reaper
{
    public class BlobBehavior : Behavior
    {
        private SoundEffect _hitSound;

        public BlobBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 3;
        public float Speed { get; set; } = 50f;
        public Vector2 Direction { get; set; } = new Vector2(0f, 1f);

        public override void Load(ContentManager contentManager)
        {
            _hitSound = contentManager.Load<SoundEffect>("audio/hit_hurt");
        }

        public override void OnOwnerCreated()
        {
            Owner.GetBehavior<DamageableBehavior>().OnDamaged += OnDamaged;
        }

        public void OnDamaged(DamageableBehavior.DamageInfo info)
        {
            Health -= info.Amount;
            if (Health <= 0)
                Owner.Destroy();

            _hitSound.Play();
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
