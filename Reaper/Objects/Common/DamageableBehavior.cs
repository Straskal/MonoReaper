using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using System;

namespace Reaper.Objects.Common
{
    public struct Damage
    {
        public int Amount;
    }

    public struct DamageResponse
    {

    }

    public class DamageableBehavior : Behavior
    {
        private TimerBehavior _timerBehavior;
        private Effect _damagedEffect;

        public DamageableBehavior(WorldObject owner) : base(owner)
        {
        }

        public event Func<Damage, DamageResponse> OnDamaged;

        public override void Load(ContentManager contentManager)
        {
            _damagedEffect = contentManager.Load<Effect>("Shaders/SolidColor");
        }

        public override void OnOwnerCreated()
        {
            _timerBehavior = Owner.GetBehavior<TimerBehavior>();
        }

        public void Damage(Damage damage)
        {
            OnDamaged?.Invoke(damage);
            Owner.GetBehavior<SpriteSheetBehavior>().Effect = _damagedEffect;

            _timerBehavior.StartTimer(0.1f, () => Owner.GetBehavior<SpriteSheetBehavior>().Effect = null);
        }
    }
}
