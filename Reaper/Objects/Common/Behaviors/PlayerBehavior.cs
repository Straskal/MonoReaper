using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Reaper.Engine;

namespace Reaper
{
    public class PlayerBehavior : Behavior
    {
        private const float ATTACK_TIME_BUFFER = 0.5f;

        private SpriteSheetBehavior _spriteSheetBehavior;
        private SoundEffect _fireSound;
        private WorldObjectPoint _projectileSpawnPoint;

        private InputManager.AxisAction _horizontalAction;
        private InputManager.AxisAction _verticalAction;
        private InputManager.AxisAction _attackHorizontalAction;
        private InputManager.AxisAction _attackVerticalAction;

        private float _attackTimer;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 5;
        public Vector2 Direction { get; private set; } = new Vector2(0, 1f);
        public float Speed { get; set; } = 100f;

        public override void Load()
        {
            _fireSound = Layout.Content.Load<SoundEffect>("audio/fireball_shoot");
        }

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.Behaviors.Get<SpriteSheetBehavior>();
            _projectileSpawnPoint = Owner.Points.Get("projectileSpawn");

            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.GetAction<InputManager.AxisAction>("horizontal");
            _verticalAction = input.GetAction<InputManager.AxisAction>("vertical");
            _attackHorizontalAction = input.GetAction<InputManager.AxisAction>("attackHorizontal");
            _attackVerticalAction = input.GetAction<InputManager.AxisAction>("attackVertical");

            Owner.Behaviors.Get<DamageableBehavior>().OnDamaged += OnDamaged;
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MoveAndAnimate(elapsedTime);
            CheckAttack();
        }

        private void CheckAttack() 
        {
            if (IsAttacking(out Vector2 direction))
            {
                if (Game.TotalTime > _attackTimer)
                {
                    var proj = Layout.Objects.CreateProjectile(_projectileSpawnPoint.Value, direction, 200f, ignoreTags: "player");
                    proj.ZOrder = Owner.ZOrder + 1;

                    _fireSound.Play();
                    _attackTimer = Game.TotalTime + ATTACK_TIME_BUFFER;
                }
            }
        }

        private void MoveAndAnimate(float elapsedTime) 
        {
            Vector2 movement = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());
            bool isMoving = movement != Vector2.Zero;

            if (movement.Length() > 1f)
                movement.Normalize();

            if (isMoving)
                Direction = movement;

            Owner.MoveAndCollide(movement * Speed * elapsedTime, out var _);
            _spriteSheetBehavior.Play(isMoving ? "walk" : "idle");
        }

        private bool IsAttacking(out Vector2 direction) 
        {
            direction = new Vector2(_attackHorizontalAction.GetAxis(), _attackVerticalAction.GetAxis());
            return direction != Vector2.Zero;
        }

        private void OnDamaged(DamageableBehavior.DamageInfo info)
        {
            Health -= info.Amount;

            if (Health <= 0)
                Owner.Destroy();
        }
    }
}
