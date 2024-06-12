using Microsoft.Xna.Framework;
using System;

using static Adventure.Constants;

namespace Adventure.Entities
{
    public abstract class Character : Entity
    {
        public static readonly int MaxPhysicsIterations = 2;

        private Vector2 _accumulator;
        private Vector2 _precise;

        public bool IsMoving { get; private set; }

        public void SlideMove(Vector2 velocity)
        {
            _accumulator += velocity;

            if (velocity.X == 0f) _accumulator.X = 0f;
            if (velocity.Y == 0f) _accumulator.Y = 0f;

            if (MathF.Abs(_accumulator.X) >= 1f || MathF.Abs(_accumulator.Y) >= 1f)
            {
                IsMoving = true;

                var iterations = MaxPhysicsIterations;
                while (iterations-- > 0)
                {
                    // Round towards zero
                    _precise.X = MathF.Truncate(_accumulator.X);
                    _precise.Y = MathF.Truncate(_accumulator.Y);

                    var collider = World.Cast(Collider, _precise, EntityLayers.Solid, out var collision);
                    if (collider == null)
                    {
                        Move(_precise);
                        _accumulator -= _precise;
                        break;
                    }

                    if (collision.Intersection.Time >= 1f)
                    {
                        _precise = collision.Direction * collision.Intersection.Time;
                        _precise.X = MathF.Round(_precise.X);
                        _precise.Y = MathF.Round(_precise.Y);
                        Move(_precise);
                        _accumulator -= _precise;
                    }

                    if (collision.Intersection.Normal.X != 0f) _accumulator.X = 0f;
                    if (collision.Intersection.Normal.Y != 0f) _accumulator.Y = 0f;

                    OnCollision(collider.Entity, collision);

                    if (collider.Entity is Character character)
                    {
                        character.OnCollision(this, collision);
                    }
                }

                IsMoving = false;
            }
        }

        public void Move(Vector2 direction)
        {
            Position += direction;
            Position = Vector2.Round(Position);
            Collider.Update();
        }

        public virtual void OnCollision(Entity other, Engine.Collision collision)
        {
        }
    }
}
