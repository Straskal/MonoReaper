using Engine;
using Microsoft.Xna.Framework;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class KinematicEntity : Entity
    {
        private const int MAX_ITERATIONS = 2;

        private Vector2 accumulator;
        private Vector2 precise;

        public bool IsMoving { get; private set; }

        public void Collide(Vector2 velocity)
        {
            accumulator += velocity;

            if (velocity.X == 0f) 
            {
                accumulator.X = 0f;
            }

            if (velocity.Y == 0f)
            {
                accumulator.Y = 0f;
            }

            if (MathF.Abs(accumulator.X) >= 1f || MathF.Abs(accumulator.Y) >= 1f)
            {
                IsMoving = true;
                
                var iterations = MAX_ITERATIONS;
                while (iterations-- > 0)
                {
                    precise.X = MathF.Round(accumulator.X, MidpointRounding.ToZero);
                    precise.Y = MathF.Round(accumulator.Y, MidpointRounding.ToZero);

                    var other = Collider.Cast(precise, EntityLayers.Solid, out var collision);
                    if (other == null)
                    {
                        Collider.Move(precise);
                        accumulator -= precise;
                        break;
                    }

                    if (collision.Intersection.Time >= 1f)
                    {
                        var preciseTime = MathF.Round(collision.Intersection.Time, MidpointRounding.ToZero);
                        precise = collision.Direction * preciseTime;

                        Collider.Move(precise);
                        accumulator -= precise;
                    }

                    if (collision.Intersection.Normal.X != 0f)
                    {
                        accumulator.X = 0f;
                    }

                    if (collision.Intersection.Normal.Y != 0f)
                    {
                        accumulator.Y = 0f;
                    }

                    OnCollision(other.Entity, collision);
                    other.Entity.OnCollision(this, collision);
                }

                IsMoving = false;
            }
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            base.DebugDraw(renderer, gameTime);

            renderer.DrawString(Store.Fonts.Default, Position.ToString(), Position.X, Position.Y, Color.White);
        }
    }
}
