using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Engine.Singletons;
using Reaper.Objects.Common;
using System;
using System.Linq;

namespace Reaper.Objects.Player
{
    public class PlayerBehavior : Behavior
    {
        private SpriteSheetBehavior _animationBehavior;
        private PlatformerBehavior _platformerBehavior;

        private Action<float> _currentState;

        private Vector2 _groundBelow;
        private bool _wasMirrored;

        private SoundEffect _jumpSound;
        private SoundEffect _hitSound;
        private SoundEffect _swingSound;

        private Input.AxisAction _moveAction;
        private Input.PressedAction _jumpAction;
        private Input.PressedAction _attackAction;
        private Input.PressedAction _toggleFullscreenAction;
        private Input.PressedAction _exitGameAction;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public override void Load(ContentManager contentManager)
        {
            _jumpSound = contentManager.Load<SoundEffect>("sounds/jump8");
            _hitSound = contentManager.Load<SoundEffect>("sounds/hi");
            _swingSound = contentManager.Load<SoundEffect>("sounds/swing");
        }

        public override void OnOwnerCreated()
        {
            _animationBehavior = Owner.GetBehavior<SpriteSheetBehavior>();
            _platformerBehavior = Owner.GetBehavior<PlatformerBehavior>();

            var input = Game.Singletons.Get<Input>();

            _moveAction = input.GetAction<Input.AxisAction>("move");
            _jumpAction = input.GetAction<Input.PressedAction>("jump");
            _attackAction = input.GetAction<Input.PressedAction>("attack");
            _toggleFullscreenAction = input.GetAction<Input.PressedAction>("toggleFullscreen");
            _exitGameAction = input.GetAction<Input.PressedAction>("exitGame");

            Owner.Layout.Zoom = 0.8f;

            GoToIdle();
        }

        public override void Tick(GameTime gameTime)
        {
            if (_toggleFullscreenAction.WasPressed())
                Owner.Layout.Game.ToggleFullscreen();

            if (_exitGameAction.WasPressed())
                Owner.Layout.Game.Exit();

            _currentState.Invoke((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void PostTick(GameTime gameTime)
        {
            UpdateFallRespawnPosition();
            HandleFallingOutsideLayout();
            ScrollLayout();
        }

        private void HandleFallingOutsideLayout()
        {
            if (Owner.Position.Y > Owner.Layout.Height)
            {
                Owner.Position = _groundBelow + new Vector2(_wasMirrored ? 32 : -32, 0);
                Owner.UpdateBBox();
            }
        }

        private void UpdateFallRespawnPosition()
        {
            var groundRay = new Rectangle(
                (int)Owner.Position.X - Owner.Origin.X,
                (int)Owner.Position.Y,
                16, 128);

            var ground = Owner.Layout.QueryBounds(groundRay)
                .Where(wo => wo != Owner && wo.IsSolid)
                .OrderBy(wo => Vector2.Distance(Owner.Position, wo.Position))
                .FirstOrDefault();

            if (ground != null)
            {
                _wasMirrored = Owner.IsMirrored;
                _groundBelow.X = Owner.Position.X;
                _groundBelow.Y = ground.Bounds.Top;
            }
        }

        private void GoToIdle()
        {
            _animationBehavior.Play("Idle");
            _currentState = Idle;
        }

        private void Idle(float elapesedTime)
        {
            if (IsMovementInputDown())
            {
                GoToMove();
            }
            else if (_platformerBehavior.IsOnGround() && IsJumpPressed())
            {
                GoToJump();
            }
            else if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
            else if (IsAttackPressed())
            {
                GoToAttack();
            }
        }

        private bool IsMovementInputDown() 
        {
            return _moveAction.GetAxis() != 0f;
        }

        private void GoToMove()
        {
            _animationBehavior.Play("Run");
            _currentState = Move;
        }

        private void Move(float elapesedTime)
        {
            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (movement == 0f)
            {
                GoToIdle();
            }
            if (_platformerBehavior.IsOnGround() && IsJumpPressed())
            {
                GoToJump();
            }
            else if (IsAttackPressed())
            {
                GoToAttack();
            }
        }

        private void GoToJump()
        {
            _scrollSmoothing = 0.25f;
            _jumpSound.Play();
            _platformerBehavior.Jump();
            _animationBehavior.Play("Jump");
            _currentState = Jump;
        }

        private void Jump(float elapesedTime)
        {
            var keyboardState = Keyboard.GetState();
            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _platformerBehavior.Jump();
            }

            _platformerBehavior.Move(movement);

            if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                _platformerBehavior.GravityAcceleration = 0;
                _platformerBehavior.Velocity = Vector2.Zero;

                GoToAttack();
            }
        }

        private void GoToFall()
        {
            _animationBehavior.Play("Fall");
            _currentState = Fall;
        }

        private void Fall(float elapesedTime)
        {
            var keyboardState = Keyboard.GetState();

            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (_platformerBehavior.IsOnGround())
            {
                _scrollSmoothing = DEFAULT_SMOOTHING;

                if (_platformerBehavior.IsMoving())
                {
                    GoToMove();
                }
                else
                {
                    GoToIdle();
                }
            }
            else if (IsAttackPressed())
            {
                _platformerBehavior.GravityAcceleration = 0;
                _platformerBehavior.Velocity = Vector2.Zero;

                GoToAttack();
            }
        }

        private const int MAX_COSECUTIVE_ATTACKS = 3;
        private int _attackIndex;
        private bool _hasCheckedForHits;

        private void GoToAttack()
        {
            _platformerBehavior.Freeze();
            _hasCheckedForHits = false;
            _animationBehavior.PlayFromBeginning($"attack_{_attackIndex}");
            _currentState = Attack;
            _swingSound.Play();
        }

        private void Attack(float elapsedTime)
        {
            if (_animationBehavior.CurrentFrame == 2 && !_hasCheckedForHits)
            {
                var bounds = new Rectangle(
                    Owner.IsMirrored ? Owner.Bounds.Left - 16 : Owner.Bounds.Right + 16,
                    (int)Math.Round(Owner.Position.Y - 16),
                    16, 16);

                var overlaps = Owner.Layout.QueryBounds(bounds);

                foreach (var overlap in overlaps)
                {
                    if (overlap == Owner)
                        continue;

                    var damageable = overlap.GetBehavior<DamageableBehavior>();

                    if (damageable == null)
                        continue;

                    damageable.Damage(new Damage { Amount = 1 });
                }

                if (overlaps.Any())
                    _hitSound.Play();

                _hasCheckedForHits = true;
            }
            else if (_animationBehavior.CurrentFrame > 3 && _attackIndex < MAX_COSECUTIVE_ATTACKS - 1 && IsAttackPressed())
            {
                _attackIndex++;
                GoToAttack();
            }
            else if (_animationBehavior.IsFinished)
            {
                _platformerBehavior.Unfreeze();
                _attackIndex = 0;
                GoToIdle();
            }
        }

        private bool IsAttackPressed()
        {
            return _attackAction.WasPressed();
        }

        private bool IsJumpPressed()
        {
            return _jumpAction.WasPressed();
        }

        private const float DEFAULT_SMOOTHING = 0.3f;
        private float _scrollSmoothing = 0.3f;

        private void ScrollLayout() 
        {
            Owner.Layout.Position = new Vector2(MathHelper.SmoothStep(Owner.Layout.Position.X, Owner.DrawPosition.X, _scrollSmoothing), Owner.Position.Y);
        }
    }
}
