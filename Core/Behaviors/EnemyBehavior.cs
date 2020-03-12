using ItsGood;

namespace Core
{
    public class EnemyBehavior : Behavior, DamageableBehavior.IDamageableCallback
    {
        private int _health = 3;

        public override void OnOwnerCreated()
        {
            Owner.GetBehavior<DamageableBehavior>().SetCallback(this);
        }

        public void OnDamaged(int amount)
        {
            _health -= amount;

            if (_health <= 0)
                Owner.Layout.DestroyObject(Owner);
        }
    }
}
