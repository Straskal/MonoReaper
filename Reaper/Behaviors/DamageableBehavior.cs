using Reaper.Engine;
using Reaper.Engine.Builtins;

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

        public DamageableBehavior(WorldObject owner) : base(owner)
        {
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
            //Owner.IsEffectEnabled = true;

            //_timerBehavior.StartTimer(0.1f, () => Owner.IsEffectEnabled = false);

            _damageableCallback?.OnDamaged(amount);
        }
    }
}
