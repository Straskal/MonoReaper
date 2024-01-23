﻿using Engine;
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

        public void SlideMove(Vector2 velocity)
        {
            accumulator += velocity;

            if (velocity.X == 0f) accumulator.X = 0f;
            if (velocity.Y == 0f) accumulator.Y = 0f;

            if (MathF.Abs(accumulator.X) >= 1f || MathF.Abs(accumulator.Y) >= 1f)
            {
                IsMoving = true;

                var iterations = MAX_ITERATIONS;
                while (iterations-- > 0)
                {
                    // Round towards zero
                    precise.X = MathF.Truncate(accumulator.X);
                    precise.Y = MathF.Truncate(accumulator.Y);

                    var collider = World.Cast(Collider, precise, EntityLayers.Solid, out var collision);
                    if (collider == null)
                    {
                        Move(precise);
                        accumulator -= precise;
                        break;
                    }

                    if (collision.Intersection.Time >= 1f)
                    {
                        precise = collision.Direction * collision.Intersection.Time;
                        precise.X = MathF.Round(precise.X);
                        precise.Y = MathF.Round(precise.Y);
                        Move(precise);
                        accumulator -= precise;
                    }

                    if (collision.Intersection.Normal.X != 0f) accumulator.X = 0f;
                    if (collision.Intersection.Normal.Y != 0f) accumulator.Y = 0f;

                    OnCollision(collider.Entity, collision);

                    if (collider.Entity is KinematicEntity kinematic)
                    {
                        kinematic.OnCollision(this, collision);
                    }
                }

                IsMoving = false;
            }
        }

        public void OverlapTriggers()
        {
            foreach (var trigger in World.OverlapEntities<Trigger>(Collider, EntityLayers.Trigger))
            {
                trigger.OnTouch(this);
            }
        }

        public void Move(Vector2 direction)
        {
            Position += direction;
            Position = Vector2.Round(Position);
            Collider.Update();
        }

        public virtual void OnCollision(Entity other, Collision collision)
        {
        }
    }
}
