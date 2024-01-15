using Engine;
using Microsoft.Xna.Framework;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class Actor : Entity
    {
        private Vector2 movementAccumulator;

        public bool IsMoving { get; private set; }

        public void Collide(Vector2 movement)
        {
            if (movement == Vector2.Zero) 
            {
                movementAccumulator.X = 0f;
                movementAccumulator.Y = 0f;
                return;
            }
            movementAccumulator += movement;
            if (MathF.Abs(movementAccumulator.X) < 1f && MathF.Abs(movementAccumulator.Y) < 1f)
            {
                return;
            }
            IsMoving = true;
            var iterations = 2;
            while (iterations-- > 0)
            {
                var move = movementAccumulator;
                // Round to nearsest whole movement
                move.X = MathF.Round(move.X, MidpointRounding.ToZero);
                move.Y = MathF.Round(move.Y, MidpointRounding.ToZero);
                var other = Collider.Cast(move, EntityLayers.Solid, out Collision collision);
                if (other == null)
                {
                    // If no collision, just move
                    Collider.Move(move);
                    movementAccumulator -= move;
                    break;
                }
                // Only move if theres room for a full pixel
                if (collision.Intersection.Time >= 1f)
                {
                    move = collision.Direction * collision.Intersection.Time;
                    move.X = MathF.Round(move.X, MidpointRounding.ToEven);
                    move.Y = MathF.Round(move.Y, MidpointRounding.ToEven);
                    Collider.Move(move);
                }
                // Cancel out velocity on colliding axis to cause slide
                if (collision.Intersection.Normal.X != 0f)
                {
                    movementAccumulator.X = 0f;
                }
                if (collision.Intersection.Normal.Y != 0f)
                {
                    movementAccumulator.Y = 0f;
                }
                // Notify entities
                OnCollision(other.Entity, collision);
                other.Entity.OnCollision(this, collision);
            }
            IsMoving = false;
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            base.DebugDraw(renderer, gameTime);
            renderer.DrawString(Store.Fonts.Default, Position.ToString(), Position.X, Position.Y, Color.White);
        }
    }
}
