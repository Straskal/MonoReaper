using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine;
using Reaper.Engine.AABB;
using Reaper.Engine.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public override void OnSpawn()
        {
            body = Entity.GetComponentOrThrow<Body>();
            animation = Entity.GetComponentOrThrow<Animator>();
            moveX = Input.NewAxisAction(Keys.A, Keys.D);
            moveY = Input.NewAxisAction(Keys.W, Keys.S);
            interact = Input.NewPressedAction(Keys.E);

            body.Acceleration = 500f;
            body.Drag = 1f;
            body.MaxSpeed = 1000f;
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

            body.Move(movementInput * delta, collision => 
            {
                if (collision.Other.Entity.TryGetComponent<LevelTrigger>(out var transition)) 
                {
                    App.Current.LoadOgmoLayout(transition.LevelName, transition.SpawnPoint);
                }

                return collision.Slide();
            });

            Animate(faceDirection);

            if (interact.WasPressed()) 
            {
                var ray = faceDirection * 50f;
                var broadphase = Collision.GetBroadphaseRectangle(Entity.Position, ray);
                var others = Level.Partition.QueryBounds(broadphase).Except(new List<Box>() { body });
                var hit = Collision.TestRay(Entity.Position, ray, others, out var info);

                if (hit) 
                {
                    var other = info.Other;
                    var fire = new Entity() { Origin = Origin.BottomCenter };

                    fire.AddComponent(new OnFire(other, 10f));
                    fire.AddComponent(new Sprite("art/player/fire"));
                    fire.AddComponent(new Animator(new[] 
                    {
                        new Animator.Animation 
                        {
                            Name = "idle",
                            ImageFilePath = "art/player/fire",
                            Loop = true,
                            SecPerFrame = 0.2f,
                            Frames = new Rectangle[] 
                            {
                                new Rectangle(0, 0, 16, 16),
                                new Rectangle(16, 0, 16, 16)
                            }
                        }
                    } ));
                    Level.Spawn(fire);
                }

                Debug.WriteLine(hit);
            }
        }

        private void Animate(Vector2 movementInput)
        {
            if (Math.Abs(movementInput.X) > 0.0005f)
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
