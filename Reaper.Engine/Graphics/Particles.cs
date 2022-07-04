
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
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

        private readonly static Random Rng = new();

        private readonly string _texturePath;
        private readonly List<Particle> _particles = new();

        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        public Particles()
        {
            IsTickEnabled = true;
            IsDrawEnabled = true;
        }

        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; } = 10f;
        public Color Color { get; set; } = Color.Yellow;
        public float Time { get; set; } = 1f;

        public Particles(string texturePath, Rectangle sourceRectangle) 
        {
            _texturePath = texturePath;
            _sourceRectangle = sourceRectangle;

            IsTickEnabled = true;
            IsDrawEnabled = true;
        }

        public override void OnLoad(ContentManager content)
        {
            if (!string.IsNullOrEmpty(_texturePath))
            {
                _texture = content.Load<Texture2D>(_texturePath);
            }
            else 
            {
                var blank = Renderer.BlankTexture;
                _texture = blank;
                _sourceRectangle = new Rectangle(0, 0, blank.Width, blank.Height);
            }
        }

        public override void OnPostTick(GameTime gameTime)
        {
            var max = 500;
            var count = _particles.Count;
            var diff = max - count;

            if (diff > 0) 
            {
                GenerateParticles(1);
            }

            var dt = gameTime.GetDeltaTime();

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
                    particle.Color = Color * (particle.Time / Time);
                    particle.Position += particle.Velocity * dt;
                    particle.Angle += particle.AngularVelocity * dt;

                    _particles[i] = particle;
                }   
            }
        }

        public override void OnDraw()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                Renderer.Draw(_texture, particle.Position, particle.Color);
            }
        }

        private void GenerateParticles(int count) 
        {
            Vector2 origin = Entity.Position;

            for (int i = 0; i < count; i++) 
            {
                var particle = new Particle
                {
                    Color = Color,
                    Position = origin,
                    Velocity = new Vector2(
                                    1f * (float)(Rng.NextDouble() * 50 - 25),
                                    1f * (float)(Rng.NextDouble() * 50 - 25)),
                    Angle = 0f,
                    AngularVelocity = 0.8f * (float)(Rng.NextDouble() * 2 - 1),
                    Time = Time
                };

                _particles.Add(particle);
            }
        }
    }
}
