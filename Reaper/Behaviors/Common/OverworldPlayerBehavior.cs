using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine;
using Reaper.Singletons;

namespace Reaper.Behaviors.Common
{
    public class OverworldPlayerBehavior : Behavior
    {
        private SpriteSheetBehavior _spriteSheetBehavior;
        private InputManager.AxisAction _horizontalAction;
        private InputManager.AxisAction _verticalAction;

        public OverworldPlayerBehavior(WorldObject owner) : base(owner) { }

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.GetBehavior<SpriteSheetBehavior>();
            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.NewAxisAction("horizontal");
            _verticalAction = input.NewAxisAction("vertical");
            _horizontalAction.AddKeys(Keys.A, Keys.D);
            _verticalAction.AddKeys(Keys.W, Keys.S);
        }

        public override void Tick(GameTime gameTime)
        {
            var movement = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());
            if (movement.Length() > 1f) movement.Normalize();
            Owner.MoveXAndCollide(movement.X, out var _);
            Owner.MoveYAndCollide(movement.Y, out var _);
            if (movement == Vector2.Zero) _spriteSheetBehavior.Play("idle");
            else _spriteSheetBehavior.Play("walk");
        }
    }
}
