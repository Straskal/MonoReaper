using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Objects.Common;
using Reaper.Singletons;

namespace Reaper.Behaviors.Common
{
    public class PlayerBehavior : Behavior
    {
        private SpriteSheetBehavior _spriteSheetBehavior;
        private InputManager.AxisAction _horizontalAction;
        private InputManager.AxisAction _verticalAction;
        private InputManager.PressedAction _attackAction;

        private Vector2 _velocity;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public Vector2 Direction { get; private set; } = Reaper.Direction.Down;
        public float Acceleration { get; set; } = 30f;
        public float MaxSpeed { get; set; } = 5f;
        public float Drag { get; set; } = 0.7f;

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.GetBehavior<SpriteSheetBehavior>();

            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.GetAction<InputManager.AxisAction>("horizontal");
            _verticalAction = input.GetAction<InputManager.AxisAction>("vertical");
            _attackAction = input.GetAction<InputManager.PressedAction>("attack");
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var movement = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());

            if (movement != Vector2.Zero)
                Direction = movement;

            if (movement.Length() > 1f)
                movement.Normalize();

            _velocity += movement * Acceleration *  elapsedTime;
            _velocity *= Drag;
            _velocity = Vector2.Clamp(_velocity, new Vector2(-MaxSpeed), new Vector2(MaxSpeed));

            Owner.MoveXAndCollide(_velocity.X, out var _);
            Owner.MoveYAndCollide(_velocity.Y, out var _);

            _spriteSheetBehavior.Play(movement == Vector2.Zero ? "idle" : "walk");

            if (_attackAction.WasPressed()) 
            {
                var proj = Layout.Spawn(Projectile.Definition(), Owner.Position);
                proj.GetBehavior<ProjectileBehavior>().Direction = movement + Direction;
                proj.GetBehavior<ProjectileBehavior>().Speed = 100f;
            }
        }
    }
}
