using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Core;
using Core.Collision;
using Core.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Reaper.Components
{
    public class Player : Component
    {
        public const float DRAG = 0.85f;
        public const float ACCELERATION = 20f;
        public const float MAX_SPEED = 0.85f;

        private Body _body;
        private Animator _animation;

        private AxisAction _moveX;
        private AxisAction _moveY;
        private PressedAction _interact;

        private Vector2 _direction = Vector2.One;
        private Vector2 _velocity = Vector2.Zero;

        public override void OnSpawn()
        {
            _body = Entity.RequireComponent<Body>();
            _animation = Entity.RequireComponent<Animator>();
            _moveX = Input.NewAxisAction(Keys.A, Keys.D);
            _moveY = Input.NewAxisAction(Keys.W, Keys.S);
            _interact = Input.NewPressedAction(Keys.E);
        }

        public override void OnTick(GameTime gameTime)
        {
            var delta = gameTime.GetDeltaTime();
            var movementInput = new Vector2(_moveX.GetAxis(), _moveY.GetAxis());
            var length = movementInput.LengthSquared();

            if (length > 0f)
            {
                if (length > 1f) movementInput.Normalize();

                _direction = movementInput;

                _animation.CurrentAnimation.Loop = true;
            }
            else
            {
                _animation.CurrentAnimation.Loop = false;
            }

            _velocity += movementInput * ACCELERATION * delta;
            _velocity *= DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_SPEED, MAX_SPEED);
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -MAX_SPEED, MAX_SPEED);

            _body.Move(ref _velocity, HandleCollision);

            Animate(_direction);

            if (_interact.WasPressed())
            {
                var fireball = new Entity(Origin.Center);

                fireball.AddComponent(new Projectile(_direction * 100f * gameTime.GetDeltaTime()));
                fireball.AddComponent(new Particles
                {
                    Velocity = -_direction,
                    AngularVelocity = 10f,
                    Color = Color.Red,
                    Time = 1f
                });

                Level.Spawn(fireball, Entity.Position);
            }
        }

        private Vector2 HandleCollision(Hit hit)
        {
            if (hit.Other.Entity.TryGetComponent<LevelTrigger>(out var transition))
            {
                App.Current.LoadOgmoLayout(transition.LevelName, transition.SpawnPoint);
            }

            return hit.Slide();
        }

        private void Animate(Vector2 movementInput)
        {
            if (Math.Abs(movementInput.X) > Math.Abs(movementInput.Y))
            {
                if (movementInput.X < 0f)
                {
                    _animation.Play("walk_left");
                }
                else
                {
                    _animation.Play("walk_right");
                }
            }
            else
            {
                if (movementInput.Y < 0f)
                {
                    _animation.Play("walk_up");
                }
                else
                {
                    _animation.Play("walk_down");

                }
            }
        }
    }
}
