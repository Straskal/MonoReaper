using System.Collections.Generic;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public sealed class Particles : GraphicsComponent
    {
        private struct Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Time;
            public float Angle;
            public float AngularVelocity;
            public Color Color;
            public float Scale;
        }

        private readonly List<Particle> particles = new();

        public Particles(Entity entity, Texture2D texture, Rectangle sourceRectangle) 
            : base(entity)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
        }

        public Texture2D Texture { get; }
        public Rectangle SourceRectangle { get; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public int MaxParticles { get; set; } = 10;
        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }
        public float MaxTime { get; set; }

        public override void PostUpdate(GameTime gameTime)
        {
            var dt = gameTime.GetDeltaTime();
            var diff = MaxParticles - particles.Count;

            if (diff > 0)
            {
                GenerateParticle();
            }

            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];

                particle.Time -= dt;

                if (particle.Time <= 0f)
                {
                    particles.RemoveAt(i);
                    i--;
                }
                else
                {
                    particle.Color = Color.Lerp(MaxColor, MinColor, particle.Time / MaxTime);
                    particle.Position += particle.Velocity * dt;
                    particle.Angle += particle.AngularVelocity * dt;
                    particles[i] = particle;
                }
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            foreach (var particle in particles)
            {
                renderer.Draw(Texture, particle.Position, SourceRectangle, particle.Color);
            }
        }

        private void GenerateParticle()
        {
            var origin = Entity.Position;

            var vx = (float)(App.Random.NextDouble() * Velocity.X - Velocity.X / 2);
            var vy = (float)(App.Random.NextDouble() * Velocity.Y - Velocity.Y / 2);

            var particle = new Particle
            {
                Color = MinColor,
                Position = origin,
                Velocity = new Vector2(vx, vy),
                Angle = 0f,
                AngularVelocity = 0,
                Time = MaxTime
            };

            particles.Add(particle);
        }
    }
}
