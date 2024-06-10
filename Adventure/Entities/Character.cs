//using Engine;
//using Engine.Extensions;
//using Microsoft.Xna.Framework;
//using System;
//using static Adventure.Constants;

//namespace Adventure.Entities
//{
//    public class Character : Entity
//    {
//        public static readonly int MaxPhysicsIterations = 2;
//        public static readonly Vector2 Gravity = new(0f, 40f);

//        protected Vector2 velocity;
//        private Vector2 accumulator;
//        private Vector2 precise;
//        private float jumpTime;

//        public bool IsMoving { get; private set; }
//        public bool IsJumping { get; private set; }
//        public bool IsTouchingWall { get; set; }
//        public bool IsGrounded { get; private set; }
//        public float JumpForce { get; set; } = 3f;
//        public float FallMultiplier { get; set; } = 3.8f;

//        public void Jump()
//        {
//            if (IsGrounded && !IsJumping)
//            {
//                velocity.Y -= JumpForce;
//                IsJumping = true;
//                jumpTime = 0f;
//            }
//        }

//        public override void Update(GameTime gameTime)
//        {
//            if (IsJumping)
//            {
//                jumpTime += gameTime.GetDeltaTime();
//                velocity.Y = Curve(velocity.Y, velocity.Y - JumpForce * 1.75f * (1f - jumpTime), 1f - jumpTime, 0.5f);
//            }

//            if (jumpTime >= 1f)
//            {
//                IsJumping = false;
//            }

//            if (!IsGrounded)
//            {
//                if (IsTouchingWall)
//                {
//                    velocity.Y += Gravity.Y * 0.5f * gameTime.GetDeltaTime();
//                }
//                else
//                {
//                    velocity.Y += Gravity.Y * FallMultiplier * gameTime.GetDeltaTime();
//                }
//            }

//            if (velocity.Y < 0f)
//            {
//                IsGrounded = false;
//            }

//            accumulator += velocity;

//            if (MathF.Abs(accumulator.X) >= 1f || MathF.Abs(accumulator.Y) >= 1f)
//            {
//                IsMoving = true;
//                IsTouchingWall = false;

//                var iterations = MaxPhysicsIterations;
//                while (iterations-- > 0)
//                {
//                    // Round towards zero
//                    precise.X = MathF.Truncate(accumulator.X);
//                    precise.Y = MathF.Truncate(accumulator.Y);

//                    var collider = Adventure.Instance.Collision.Cast(Collider, precise, EntityLayers.Solid, out var collision);
//                    if (collider == null)
//                    {
//                        Move(precise);
//                        accumulator -= precise;
//                        break;
//                    }

//                    if (collision.Intersection.Time >= 1f)
//                    {
//                        precise = collision.Direction * collision.Intersection.Time;
//                        precise.X = MathF.Round(precise.X);
//                        precise.Y = MathF.Round(precise.Y);
//                        Move(precise);
//                        accumulator -= precise;
//                    }

//                    if (collision.Intersection.Normal.Y == -1f)
//                    {
//                        IsGrounded = true;
//                        IsJumping = false;
//                    }

//                    if (collision.Intersection.Normal.X != 0f)
//                    {
//                        IsTouchingWall = true;
//                        velocity.Y = MathF.Max(velocity.Y, 0f);
//                        accumulator.Y = MathF.Max(accumulator.Y, 0f);
//                    }

//                    if (collision.Intersection.Normal.X != 0f) velocity.X = 0f;
//                    if (collision.Intersection.Normal.Y != 0f) velocity.Y = 0f;
//                    if (collision.Intersection.Normal.X != 0f) accumulator.X = 0f;
//                    if (collision.Intersection.Normal.Y != 0f) accumulator.Y = 0f;

//                    OnCollision(collider.Entity, collision);

//                    if (collider.Entity is Character kinematic)
//                    {
//                        kinematic.OnCollision(this, collision);
//                    }
//                }

//                IsMoving = false;
//            }
//        }

//        public void Move(Vector2 direction)
//        {
//            Position += direction;
//            Position = Vector2.Round(Position);
//            Collider.Update();
//        }

//        public virtual void OnCollision(Entity other, Engine.Collision collision)
//        {
//        }

//        public static float Curve(float min, float max, float percent, float curve)
//        {
//            var linear = MathHelper.Lerp(min, max, percent);
//            var percentCurve = MathHelper.Lerp(curve, 0, MathF.Abs(percent - 0.5f) * 2f);

//            if (curve > 0f)
//            {
//                return MathHelper.Lerp(linear, max, percentCurve);
//            }
//            else
//            {
//                return MathHelper.Lerp(linear, min, -percentCurve);
//            }
//        }
//    }
//}
