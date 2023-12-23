using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    /// <summary>
    /// This component will add particles to an entity.
    /// </summary>
    public sealed class Particles : Component
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

        private readonly List<Particle> _particles = new();

        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        public Particles(Texture2D texture, Rectangle sourceRectangle)
        {
            _texture = texture;
            _sourceRectangle = sourceRectangle;
        }

        /// <summary>
        /// The max velocity that a particle can travel at.
        /// </summary>
        public Vector2 Velocity 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The max angular velocity that a prticle can travel at.
        /// </summary>
        public float AngularVelocity 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The max number of particles that can be emitted.
        /// </summary>
        public int MaxParticles 
        { 
            get; 
            set; 
        } = 10;

        /// <summary>
        /// The color of the particle at the beginning of it's lifespan.
        /// </summary>
        public Color MinColor 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The color of the particle at the ending of it's lifespan.
        /// </summary>
        public Color MaxColor 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The max time that a particle can live.
        /// </summary>
        public float MaxTime 
        { 
            get; 
            set; 
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var dt = gameTime.GetDeltaTime();
            var diff = MaxParticles - _particles.Count;

            if (diff > 0)
            {
                GenerateParticle();
            }

            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                particle.Time -= dt;

                if (particle.Time <= 0f)
                {
                    _particles.RemoveAt(i);
                    i--;
                }
                else
                {
                    particle.Color = Color.Lerp(MaxColor, MinColor, particle.Time / MaxTime);
                    particle.Position += particle.Velocity * dt;
                    particle.Angle += particle.AngularVelocity * dt;
                    _particles[i] = particle;
                }
            }
        }

        public override void OnDraw(Renderer renderer, GameTime gameTime)
        {
            foreach (var particle in _particles) 
            {
                renderer.Draw(_texture, particle.Position, _sourceRectangle, particle.Color);
            }
        }

        private void GenerateParticle()
        {
            var origin = Entity.Position;

            var vx = (float)(App.Instance.Random.NextDouble() * Velocity.X - Velocity.X / 2);
            var vy = (float)(App.Instance.Random.NextDouble() * Velocity.Y - Velocity.Y / 2);

            var particle = new Particle
            {
                Color = MinColor,
                Position = origin,
                Velocity = new Vector2(vx, vy),
                Angle = 0f,
                AngularVelocity = 0,
                Time = MaxTime
            };

            _particles.Add(particle);
        }
    }
}
