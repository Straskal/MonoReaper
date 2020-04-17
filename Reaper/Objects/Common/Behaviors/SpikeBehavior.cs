using Microsoft.Xna.Framework;
using Reaper.Engine;
using System;

namespace Reaper
{
    public class SpikeBehavior : Behavior
    {
        private const float DOWN_TIME = 3f;
        private const float UP_TIME = 1f;

        private SpriteSheetBehavior _spriteSheet;
        private Action<GameTime> _currentAction;
        private float _timer;

        public SpikeBehavior(WorldObject owner) : base(owner) { _currentAction = GoDown; }

        public override void OnOwnerCreated()
        {
            _spriteSheet = Owner.Behaviors.Get<SpriteSheetBehavior>();
        }

        public override void Tick(GameTime gameTime)
        {
            _currentAction?.Invoke(gameTime);
        }

        public void GoDown(GameTime gameTime)
        {
            _timer = (float)gameTime.TotalGameTime.TotalSeconds + DOWN_TIME;
            _spriteSheet.Play("go_down");
            _currentAction = Down;
        }

        public void Down(GameTime gameTime) 
        {
            if ((float)gameTime.TotalGameTime.TotalSeconds > _timer)
                _currentAction = GoUp;
        }

        public void GoUp(GameTime gameTime)
        {
            _timer = (float)gameTime.TotalGameTime.TotalSeconds + UP_TIME;
            _spriteSheet.Play("go_up");
            _currentAction = Up;
        }

        public void Up(GameTime gameTime)
        {
            var overlaps = Layout.Grid.QueryBounds(Owner.Bounds, ignoreTags: "spikes");

            foreach (var overlap in overlaps) 
            {
                if (overlap.WorldObject.Behaviors.TryGet<DamageableBehavior>(out var damageable))
                {
                    damageable.Damage(new DamageableBehavior.DamageInfo { Amount = 1 });
                }
            }

            if ((float)gameTime.TotalGameTime.TotalSeconds > _timer)
                _currentAction = GoDown;
        }
    }
}
