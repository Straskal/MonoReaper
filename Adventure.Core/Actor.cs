using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Adventure.Core
{
    public abstract class Actor
    {
        private const int CollisionDetectionIterations = 2;

        private Vector2 _position;
        private Vector2 _movementAccumulator;
        private CollisionShape _shapeLocal;
        private CollisionShape _shape;

        public Adventure Game { get; internal set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _shape.Position = _shapeLocal.Position + Position;
            }
        }

        public CollisionShape ShapeLocal
        {
            get => _shapeLocal;
            protected set
            {
                _shapeLocal = value;
                _shape = _shapeLocal;
                _shape.Position += Position;
            }
        }

        public CollisionShape Shape => _shape;
        public CollisionLayers LayerMask { get; protected set; }
        public bool OverlapsTriggers { get; protected set; }
        public bool IsMoving { get; private set; }

        public virtual void Spawn()
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Draw(float deltaTime)
        {
        }

        public virtual void DebugDraw(float deltaTime)
        {
            Shape.DebugDraw(Game.Renderer);
            Game.Renderer.DrawString(ContentCache.Fonts.Default, $"{Position.X}, {Position.Y}", Position + Vector2.UnitY * 5f, Color.White);
        }

        public virtual void HandleCollision(Actor other, Collision collision)
        {
        }

        public void SlideMove(Vector2 velocity)
        {
            _movementAccumulator += velocity;

            // If the actor doesn't have enough room to move at least one entire pixel, then bail out.
            if (Math.Abs(_movementAccumulator.X) < 1f && MathF.Abs(_movementAccumulator.Y) < 1f)
            {
                return;
            }

            IsMoving = true;

            var iterations = CollisionDetectionIterations;
            var preciseMovement = Vector2.Zero;

            while (iterations-- > 0)
            {
                preciseMovement.X = MathF.Truncate(_movementAccumulator.X);
                preciseMovement.Y = MathF.Truncate(_movementAccumulator.Y);

                var other = Game.Raycast(this, preciseMovement, CollisionLayers.Solid, out var collision);
                if (other == null)
                {
                    Position += preciseMovement;
                    _movementAccumulator -= preciseMovement;
                    break;
                }

                if (collision.Intersection.Time >= 1f)
                {
                    preciseMovement = collision.Direction * collision.Intersection.Time;
                    preciseMovement.X = MathF.Truncate(preciseMovement.X);
                    preciseMovement.Y = MathF.Truncate(preciseMovement.Y);
                    Position += preciseMovement;
                    _movementAccumulator -= preciseMovement;
                }

                if (collision.RemainingTime > 0f)
                {
                    var remainingMovement = collision.Direction * collision.RemainingTime;
                    var dot = Vector2.Dot(remainingMovement, collision.Intersection.Normal);

                    // Substitute the remaining with a new remaining amount in a direction that will slide along the normal.
                    _movementAccumulator -= remainingMovement;
                    _movementAccumulator += remainingMovement - dot * collision.Intersection.Normal;

                    // Add a small buffer to push the actor further out of the intersection point.
                    // This helps will circle collisions and smooth sliding along round edges.
                    _movementAccumulator += collision.Intersection.Normal * 0.5f;
                }

                HandleCollision(other, collision);
                other.HandleCollision(this, collision);
            }

            if (OverlapsTriggers)
            {
                foreach (var trigger in Game.Overlap<Trigger>(this, CollisionLayers.Trigger))
                {
                    trigger.OnTouch(this);
                }
            }

            IsMoving = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckLayer(CollisionLayers mask)
        {
            return (mask & LayerMask) != 0;
        }
    }
}