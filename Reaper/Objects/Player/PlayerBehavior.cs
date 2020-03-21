using Reaper.Objects;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Reaper.Objects.Common;

namespace Reaper.Objects.Player
{
    public class PlayerBehavior : Behavior
    {
        private SpriteSheetBehavior _animationBehavior;
        private PlatformerBehavior _platformerBehavior;
        private KeyboardState _previousKeyState;
        private Action<float> _currentState;
        private Vector2 _lastGroundedPosition;
        private bool _lastMirroredGrounded;
        private SoundEffect _jumpSound;
        private SoundEffect _hitSound;

        public PlayerBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void Load(ContentManager contentManager)
        {
            _jumpSound = contentManager.Load<SoundEffect>("sounds/jump8");
            _hitSound = contentManager.Load<SoundEffect>("sounds/hi");
        }

        public override void OnOwnerCreated()
        {
            _animationBehavior = Owner.GetBehavior<SpriteSheetBehavior>();
            _platformerBehavior = Owner.GetBehavior<PlatformerBehavior>();

            Owner.Layout.Zoom = 0.8f;

            GoToIdle();
        }

        private bool _spawned;

        public override void Tick(GameTime gameTime)
        {
            _currentState.Invoke((float)gameTime.ElapsedGameTime.TotalSeconds);
            _previousKeyState = Keyboard.GetState();

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Down)) 
            {
                Owner.Layout.Zoom += 0.1f;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                Owner.Layout.Zoom -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Right) && !_spawned)
            {
                Owner.Layout.Spawn(Definitions.Get("other"), Owner.Position + new Vector2(64, 0));
                _spawned = true;
            }
            else if (keyboardState.IsKeyUp(Keys.Right))
            {
                _spawned = false;
            }
        }

        public override void PostTick(GameTime gameTime)
        {
            Owner.Layout.Position = new Vector2(MathHelper.SmoothStep(Owner.Layout.Position.X, Owner.DrawPosition.X, 0.3f), Owner.Layout.Position.Y);

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
                _lastMirroredGrounded = Owner.IsMirrored;
                _lastGroundedPosition.X = Owner.Position.X;
                _lastGroundedPosition.Y = ground.Bounds.Top;
            }

            if (Owner.Position.Y > Owner.Layout.Height) 
            {
                Owner.Position = _lastGroundedPosition + new Vector2(_lastMirroredGrounded ? 32 : -32, 0);
                Owner.UpdateBBox();
            }
        }

        private void GoToIdle() 
        {
            _animationBehavior.Play("Idle");
            _currentState = Idle;
        }

        private void Idle(float elapesedTime) 
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D))
            {
                GoToMove();
            }
            else if (_platformerBehavior.IsOnGround() && keyboardState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
            {
                GoToJump();
            }
            else if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
            else if (keyboardState.IsKeyDown(Keys.Left) && _previousKeyState.IsKeyUp(Keys.Left)) 
            {
                GoToAttack();
            }
        }

        private void GoToMove()
        {
            _animationBehavior.Play("Run");
            _currentState = Move;
        }

        private void Move(float elapesedTime)
        {
            float movement = 0;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (movement == 0f) 
            {
                GoToIdle();
            }
            if (_platformerBehavior.IsOnGround() && keyboardState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
            {
                GoToJump();
            }
            else if (keyboardState.IsKeyDown(Keys.Left) && _previousKeyState.IsKeyUp(Keys.Left))
            {
                GoToAttack();
            }
        }

        private void GoToJump()
        {
            _jumpSound.Play();
            _platformerBehavior.Jump();
            _animationBehavior.Play("Jump");
            _currentState = Jump;
        }

        private void Jump(float elapesedTime)
        {
            var keyboardState = Keyboard.GetState();
            float movement = 0;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
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

            float movement = 0;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (_platformerBehavior.IsOnGround())
            {
                if (_platformerBehavior.IsMoving())
                {
                    GoToMove();
                }
                else
                {
                    GoToIdle();
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Left) && _previousKeyState.IsKeyUp(Keys.Left))
            {
                _platformerBehavior.GravityAcceleration = 0;
                _platformerBehavior.Velocity = Vector2.Zero;

                GoToAttack();
            }
        }

        private void GoToAttack() 
        {
            _platformerBehavior.Freeze();
            _hasCheckedForHits = false;
            _animationBehavior.Play("Attack");
            _currentState = Attack;
        }

        private bool _hasCheckedForHits;

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
            else if (_animationBehavior.IsFinished) 
            {
                _platformerBehavior.Unfreeze();
                GoToIdle();
            }
        }
    }
}
