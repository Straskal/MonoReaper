using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine;
using Reaper.Engine.AABB;
using Reaper.Engine.Components;

namespace Reaper.Components
{
    public class Player : Component
    {
        private Body body;
        private Animator animation;

        private AxisAction moveX;
        private AxisAction moveY;
        private PressedAction interact;

        private Vector2 faceDirection;

        private float drag = 0.85f;
        private float acceleration = 10f;
        private float maxSpeed = 0.85f;
        private Vector2 velocity = Vector2.Zero;

        public override void OnSpawn()
        {
            body = Entity.GetComponentOrThrow<Body>();
            animation = Entity.GetComponentOrThrow<Animator>();
            moveX = Input.NewAxisAction(Keys.A, Keys.D);
            moveY = Input.NewAxisAction(Keys.W, Keys.S);
            interact = Input.NewPressedAction(Keys.E);
        }

        public override void OnTick(GameTime gameTime)
        {
            var delta = gameTime.GetDeltaTime();
            var movementInput = new Vector2(moveX.GetAxis(), moveY.GetAxis());

            if (movementInput.Length() > 1f)
            {
                movementInput.Normalize();
            }

            if (movementInput.Length() > 0f)
            {
                faceDirection = movementInput;

                animation.CurrentAnimation.Loop = true;
            }
            else 
            {
                animation.CurrentAnimation.Loop = false;
            }

            velocity += movementInput * acceleration * delta;
            velocity *= drag;
            velocity.X = MathHelper.Clamp(velocity.X, -maxSpeed, maxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -maxSpeed, maxSpeed);

            body.Move(ref velocity, HandleCollision);

            Animate(velocity);
        }

        private Vector2 HandleCollision(CollisionInfo info) 
        {
            if (info.Other.Entity.TryGetComponent<LevelTrigger>(out var transition))
            {
                App.Current.LoadOgmoLayout(transition.LevelName, transition.SpawnPoint);
            }

            return info.Slide();
        }

        private void Animate(Vector2 movementInput)
        {
            if (Math.Abs(movementInput.X) > Math.Abs(movementInput.Y))
            {
                if (movementInput.X < 0f)
                {
                    animation.Play("walk_left");
                }
                else
                {
                    animation.Play("walk_right");
                }
            }
            else
            {
                if (movementInput.Y < 0f)
                {
                    animation.Play("walk_up");
                }
                else
                {
                    animation.Play("walk_down");

                }
            }
        }
    }
}
