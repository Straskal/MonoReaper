using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;
using Reaper.Engine.Behaviors;

namespace Reaper
{
    public class DamageableBehavior : Behavior
    {
        public interface IDamageableCallback 
        {
            void OnDamaged(int amount);
        }

        private TimerBehavior _timerBehavior;
        private IDamageableCallback _damageableCallback;
        private Effect _damagedEffect;

        public DamageableBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void Load(ContentManager contentManager)
        {
            _damagedEffect = contentManager.Load<Effect>("Shaders/SolidColor");
        }

        public override void OnOwnerCreated()
        {
            _timerBehavior = Owner.GetBehavior<TimerBehavior>();
        }

        public void SetCallback(IDamageableCallback callback) 
        {
            _damageableCallback = callback;
        }

        public void Damage(int amount)
        {
            Owner.GetBehavior<SpriteSheetBehavior>().Effect = _damagedEffect;

            _timerBehavior.StartTimer(0.1f, () => Owner.GetBehavior<SpriteSheetBehavior>().Effect = null);

            _damageableCallback?.OnDamaged(amount);
        }
    }
}
