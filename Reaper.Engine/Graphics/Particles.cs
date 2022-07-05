
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

        private readonly List<Particle> _particles = new();

        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        public Particles()
        {
            IsTickEnabled = true;
            IsDrawEnabled = true;
        }

        public Particles(Texture2D texture, Rectangle sourceRectangle) : this()
        {
            _texture = texture;
            _sourceRectangle = sourceRectangle;
        }

        public Vector2 MaxVelocity { get; set; }
        public float MaxAngularVelocity { get; set; }

        public int MaxParticles { get; set; } = 10;
        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }
        public float MaxTime { get; set; }

        public override void OnLoad(ContentManager content)
        {
            if (_texture == null)
            {
                _texture = Renderer.BlankTexture;
                _sourceRectangle = new Rectangle(0, 0, Renderer.BlankTexture.Width, Renderer.BlankTexture.Height);
            }
        }

        public override void OnPostTick(GameTime gameTime)
        {
            var dt = gameTime.GetDeltaTime();
            var diff = MaxParticles - _particles.Count;

            if (diff > 0)
            {
                GenerateParticles(1);
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

        public override void OnDraw()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                Renderer.Draw(_texture, _sourceRectangle, particle.Position, particle.Color, false);
            }
        }

        private void GenerateParticles(int count)
        {
            Vector2 origin = Entity.Position;

            for (int i = 0; i < count; i++)
            {
                float vx = (float)(Rng.NextDouble() * MaxVelocity.X - (MaxVelocity.X / 2));
                float vy = (float)(Rng.NextDouble() * MaxVelocity.Y - (MaxVelocity.Y / 2));

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
}
