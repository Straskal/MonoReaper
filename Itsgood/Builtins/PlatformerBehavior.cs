using ItsGood.Utils.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace ItsGood.Builtins
{
    public class PlatformerBehavior : Behavior
    {
        public interface IAnimationCallbacks
        {
            void OnMoved();
            void OnStopped();
            void OnJumped();
            void OnLanded();
        }

        private const float MOVE_ACCELERATION = 2000f;
        private const float MAX_MOVE_SPEED = 100f;
        private const float DRAG = 0.8f;
        private const float MAX_JUMP_TIME = 0.35f;
        private const float JUMP_VELOCITY = -1500;
        private const float JUMP_CONTROL = 0.14f;
        private const float GRAVITY_ACCELERATION = 1400f;
        private const float MAX_FALL_SPEED = 400f;

        private IAnimationCallbacks _receiver;

        private float _jumpTime;
        private float _movement;
        private bool _isJumping;
        private bool _wasJumping;

        private Vector2 _velocity;

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public void SetAnimationCallbacks(IAnimationCallbacks receiver)
        {
            _receiver = receiver;
        }

        public void Move(float movement)
        {
            _movement = movement;
        }

        public void Jump()
        {
            _isJumping = true;
        }

        public override void Tick(GameTime gameTime)
        {
            ApplyPhysics(gameTime);

            _movement = 0;
            _isJumping = false;
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _velocity.X += MOVE_ACCELERATION * _movement * elapsedTime;
            _velocity.X *= DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
            _velocity.Y = MathHelper.Clamp(_velocity.Y + GRAVITY_ACCELERATION * elapsedTime, -MAX_FALL_SPEED, MAX_FALL_SPEED);

            HandleJump(gameTime);

            Owner.MoveX(_velocity.X * elapsedTime);
            Owner.MoveY(_velocity.Y * elapsedTime);
            Owner.UpdateBBox();

            if (Owner.Position.Y == Owner.PreviousPosition.Y)
                _velocity.Y = 0f;
        }

        private void HandleJump(GameTime gameTime)
        {
            if (_isJumping)
            {
                // Initial jump or continuation.
                if ((!_wasJumping && IsOnGround()) || _jumpTime > 0f)
                {
                    _jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _receiver.OnJumped();
                }

                if (_jumpTime > 0f && _jumpTime <= MAX_JUMP_TIME)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    _velocity.Y = JUMP_VELOCITY * (1.0f - (float)Math.Pow(_jumpTime / MAX_JUMP_TIME, JUMP_CONTROL));
                }
                else
                {
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }

            _wasJumping = _isJumping;
        }

        private bool IsOnGround()
        {
            return Owner.Layout.Grid.TestOverlap(Owner, new Vector2(0, 1)) != null;
        }
    }
}
