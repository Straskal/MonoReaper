using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ItsGood.Builtins
{
    public class PlatformerBehavior : Behavior
    {
        public interface ICallbackReceiver 
        {
            void OnMoved();
            void OnStopped();
        }

        private const float MOVE_ACCELERATION = 1500f; 
        private const float MAX_MOVE_SPEED = 400f;
        private const float MOVE_DRAG = 0.8f;

        private ICallbackReceiver _receiver;
        private Vector2 _velocity;
        private bool _wasMoving;

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float movement = GetMovement();

            _velocity.X += MOVE_ACCELERATION * movement * elapsedTime;
            _velocity.X *= MOVE_DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);

            Owner.Position += _velocity * elapsedTime;

            if (!_wasMoving && movement != 0f) 
            {
                _wasMoving = true;
                _receiver?.OnMoved();
            }
            else if (_wasMoving && movement == 0f) 
            {
                _wasMoving = false;
                _receiver?.OnStopped();
            }
        }

        public void SetCallbackReceiver(ICallbackReceiver receiver) 
        {
            _receiver = receiver;
        }

        private float GetMovement()
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

            return movement;
        }
    }
}
