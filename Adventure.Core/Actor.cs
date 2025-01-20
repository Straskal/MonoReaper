using Engine;
using Microsoft.Xna.Framework;
using System;

namespace Adventure.Core
{
    public abstract class Actor
    {
        private Vector2 movementAccumulator;

        public Adventure Game { get; internal set; }
        public Vector2 Position { get; set; }
        public Sprite Sprite { get; set; }

        public virtual void Spawn() { }
        public virtual void Destroy() { }
        public virtual void Update() { }
        public virtual void Draw() { }

        public void Move(Vector2 velocity)
        {
            movementAccumulator += velocity;

            var movement = Vector2.Zero;

            if (Math.Abs(movementAccumulator.X) > 1f) 
            {
                movement.X = MathF.Truncate(movementAccumulator.X);
            }

            if (MathF.Abs(movementAccumulator.Y) < 1f)
            {
                movement.Y = MathF.Truncate(movementAccumulator.Y);
            }

            Position += movement;
            movementAccumulator -= movement;
        }
    }
}