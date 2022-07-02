using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;

namespace Reaper.Engine.Components
{
    public delegate Vector2 CollisionCallback(CollisionInfo hit);

    public sealed class Body : Box
    {
        private readonly List<Box> visited = new();

        public Body(float width, float height) : base(CollisionLayer.Overlap, width, height)
        {
        }

        private Vector2 velocity;
        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        private float drag = 0.8f;
        public float Drag
        {
            get => drag;
            set => drag = value;
        }

        private float acceleration = 15f;
        public float Acceleration
        {
            get => acceleration;
            set => acceleration = value;
        }

        private float maxSpeed = 1.25f;
        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        public override void OnPostTick(GameTime gameTime)
        {
            velocity *= Drag;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);
        }

        public void Move(Vector2 direction, CollisionCallback response = null)
        {
            velocity += direction * Acceleration;

            visited.Clear();
            visited.Add(this);

            while (true)
            {
                var previousPosition = Entity.Position;
                var others = Level.Partition.QueryBounds(Bounds).Except(visited);
                var collided = Collision.TestAABB(this, velocity, others, out var info);

                Entity.Position = info.Position;

                UpdateBBox(previousPosition);

                if (!collided) break;

                velocity = response?.Invoke(info) ?? Vector2.Zero;

                visited.Add(info.Other);
            }
        }
    }
}
