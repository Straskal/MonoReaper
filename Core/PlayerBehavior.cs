using ItsGood;
using ItsGood.Builtins;

namespace Core
{
    public class PlayerBehavior : Behavior, PlatformerBehavior.ICallbackReceiver
    {
        private AnimatedBehavior _animatedBehavior;

        public override void OnOwnerCreated()
        {
            _animatedBehavior = Owner.GetBehavior<AnimatedBehavior>();

            Owner.GetBehavior<PlatformerBehavior>().SetCallbackReceiver(this);
        }

        public void OnMoved()
        {
            _animatedBehavior.Play("Run");
        }

        public void OnStopped()
        {
            _animatedBehavior.Play("Idle");
        }
    }
}
