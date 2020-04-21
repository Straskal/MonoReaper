using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Reaper.Engine;

namespace Reaper
{
    public class PlayerBehavior : Behavior
    {
        private const float ATTACK_TIME_BUFFER = 0.5f;
        private const float MAX_SPEED = 15f;
        private const float ACCELERATION = 25f;
        private const float DRAG = 0.8f;

        private SpriteSheetBehavior _spriteSheetBehavior;
        private SoundEffect _fireSound;
        private WorldObjectPoint _projectileSpawnPoint;

        private AxisAction _horizontalAction;
        private AxisAction _verticalAction;
        private AxisAction _attackHorizontalAction;
        private AxisAction _attackVerticalAction;

        private Vector2 _velocity;
        private float _attackTimer;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 5;
        public Vector2 Direction { get; private set; } = new Vector2(0, 1f);

        public Vector2 Velocity 
        {
            get => _velocity;
            set => _velocity = value;
        }

        public override void Load()
        {
            _fireSound = Layout.Content.Load<SoundEffect>("audio/fireball_shoot");
        }

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.Behaviors.Get<SpriteSheetBehavior>();
            _projectileSpawnPoint = Owner.Points.Get("projectileSpawn");

            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.GetAction<AxisAction>("horizontal");
            _verticalAction = input.GetAction<AxisAction>("vertical");
            _attackHorizontalAction = input.GetAction<AxisAction>("attackHorizontal");
            _attackVerticalAction = input.GetAction<AxisAction>("attackVertical");

            Owner.Behaviors.Get<DamageableBehavior>().OnDamaged += OnDamaged;
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsMoving(out var movementDirection)) 
            {
                UpdateDirection(movementDirection);
            }

            UpdateVelocity(movementDirection, elapsedTime);
            MoveAndCollide();
            Animate(movementDirection);
            
            if (IsAttacking(out var attackDirection)) 
            {
                Attack(attackDirection);
            }
        }

        private bool IsMoving(out Vector2 movementInput) 
        {
            movementInput = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());

            if (movementInput.Length() > 1f)
                movementInput.Normalize();

            return movementInput != Vector2.Zero;
        }

        private void UpdateVelocity(Vector2 movementInput, float elapsedTime) 
        {
            _velocity += movementInput * ACCELERATION * elapsedTime;
            _velocity *= DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_SPEED, MAX_SPEED);
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -MAX_SPEED, MAX_SPEED);
        }

        private void MoveAndCollide()
        {
            if (Owner.MoveAndCollideX(_velocity.X))
                _velocity.X = 0f;

            if (Owner.MoveAndCollideY(_velocity.Y))
                _velocity.Y = 0f;
        }

        private void UpdateDirection(Vector2 movementInput) 
        {
            Direction = movementInput;
        }

        private void Animate(Vector2 movementInput)
        {
            _spriteSheetBehavior.Play(movementInput != Vector2.Zero ? "walk" : "idle");
        }

        private bool IsAttacking(out Vector2 direction)
        {
            direction = new Vector2(_attackHorizontalAction.GetAxis(), _attackVerticalAction.GetAxis());

            if (direction.Length() > 1f)
                direction.Normalize();

            return direction != Vector2.Zero;
        }

        private void Attack(Vector2 direction) 
        {
            if (Game.TotalTime > _attackTimer)
            {
                var proj = Layout.Objects.CreateProjectile(_projectileSpawnPoint.Value, direction, 200f, ignoreTags: "player");
                proj.ZOrder = Owner.ZOrder + 1;

                _fireSound.Play();
                _attackTimer = Game.TotalTime + ATTACK_TIME_BUFFER;
            }
        }

        private void OnDamaged(DamageableBehavior.DamageInfo info)
        {
            Health -= info.Amount;

            if (Health <= 0)
                Owner.Destroy();
        }
    }
}
