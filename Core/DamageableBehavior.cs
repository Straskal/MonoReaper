using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;

namespace Core
{
    public class DamageableBehavior : Behavior
    {
        private float _timer;
        private float _currentTime;
        private bool _needsTick;

        public void Damage(int amount)
        {
            _timer = _currentTime + 0.1f;
            Owner.IsEffectEnabled = true;
            _needsTick = true;
        }

        public override void Tick(GameTime gameTime)
        {
            if (!_needsTick)
                return;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentTime += elapsedTime;

            if (_currentTime > _timer)
            {
                Owner.IsEffectEnabled = false;
                _needsTick = false;
            }
        }
    }
}
