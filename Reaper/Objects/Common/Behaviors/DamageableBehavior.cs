using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;
using System;

namespace Reaper
{
    public class DamageableBehavior : Behavior
    {
        public struct DamageInfo
        {
            public int Amount;
            public Vector2 Direction;
        }

        private Effect _effect;
        private SpriteSheetBehavior _spriteSheet;

        public DamageableBehavior(WorldObject owner) : base(owner) { }

        public event Action<DamageInfo> OnDamaged;

        public override void Load(ContentManager contentManager)
        {
            _effect = contentManager.Load<Effect>("Shaders/SolidColor");
        }

        public override void OnOwnerCreated()
        {
            _spriteSheet = Owner.Behaviors.Get<SpriteSheetBehavior>();
        }

        public void Damage(DamageInfo info)
        {
            OnDamaged?.Invoke(info);
            _spriteSheet.Effect = _effect;
            Owner.Timers.Start("damaged", 0.1f, () => _spriteSheet.Effect = null);
        }
    }
}
