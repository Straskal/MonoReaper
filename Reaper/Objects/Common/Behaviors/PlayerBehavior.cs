using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine;

namespace Reaper
{
    public class PlayerBehavior : Behavior
    {
        private const float ATTACK_TIME_BUFFER = 0.5f;

        private SpriteSheetBehavior _spriteSheetBehavior;
        private SoundEffect _fireSound;
        private InputManager.AxisAction _horizontalAction;
        private InputManager.AxisAction _verticalAction;
        private InputManager.AxisAction _attackHorizontalAction;
        private InputManager.AxisAction _attackVerticalAction;

        private float _attackTimer;
        private bool _isMoving;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public int Health { get; private set; } = 5;
        public Vector2 Direction { get; private set; } = new Vector2(0, 1f);
        public float Speed { get; set; } = 100f;

        public override void Load(ContentManager contentManager)
        {
            _fireSound = contentManager.Load<SoundEffect>("audio/fireball_shoot");
        }

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.GetBehavior<SpriteSheetBehavior>();
            Owner.GetBehavior<DamageableBehavior>().OnDamaged += OnDamaged;

            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.GetAction<InputManager.AxisAction>("horizontal");
            _verticalAction = input.GetAction<InputManager.AxisAction>("vertical");
            _attackHorizontalAction = input.GetAction<InputManager.AxisAction>("attackHorizontal");
            _attackVerticalAction = input.GetAction<InputManager.AxisAction>("attackVertical");
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 movement = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());
            _isMoving = movement != Vector2.Zero;

            if (_isMoving)
                Direction = movement;

            if (movement.LengthSquared() > 0f)
                movement.Normalize();

            Owner.MoveAndCollide(movement * Speed * elapsedTime, out var _);
            CheckAttack(gameTime);
            _spriteSheetBehavior.Play(_isMoving ? "walk" : "idle");
        }

        private void CheckAttack(GameTime gameTime) 
        {
            var attackDirection = new Vector2(_attackHorizontalAction.GetAxis(), _attackVerticalAction.GetAxis());

            if (attackDirection != Vector2.Zero)
            {
                if (gameTime.TotalGameTime.TotalSeconds > _attackTimer)
                {
                    if (attackDirection.Length() > 1f)
                        attackDirection.Normalize();

                    var proj = Layout.Spawn(Projectile.Definition(), Owner.GetPoint("projectileSpawn"));
                    proj.ZOrder = Owner.ZOrder + 1;
                    proj.GetBehavior<ProjectileBehavior>().Direction = attackDirection;
                    proj.GetBehavior<ProjectileBehavior>().Speed = 200f;
                    proj.GetBehavior<ProjectileBehavior>().IgnoreTags = new[] { "player" };

                    _fireSound.Play();
                    _attackTimer = (float)gameTime.TotalGameTime.TotalSeconds + ATTACK_TIME_BUFFER;
                }
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
