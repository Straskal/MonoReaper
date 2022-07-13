using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Core.Collision;
using Core.Graphics;

using static Reaper.Constants;

namespace Reaper.Components
{
    public class Player : Component
    {
        public const float DRAG = 0.85f;
        public const float ACCELERATION = 20f;
        public const float MAX_SPEED = 0.85f;

        private Body _body;
        private Sprite _sprite;
        private SpriteSheet _spriteSheet;

        private AxisAction _moveX;
        private AxisAction _moveY;
        private PressedAction _interact;

        private Vector2 _direction = Vector2.One;
        private Vector2 _velocity = Vector2.Zero;

        public override void OnLoad(ContentManager content)
        {
            Fireball.Preload(content);

            var playerTexture = content.Load<Texture2D>("art/player/player");

            Entity.AddComponent(_body = new Body(12, 16, EntityLayers.Player));
            Entity.AddComponent(_sprite = new Sprite(playerTexture) { ZOrder = 10 });
            Entity.AddComponent(_spriteSheet = new SpriteSheet(new[]
            {
                new SpriteSheet.Animation
                {
                    Name = "idle",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(0, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_down",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_up",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 4, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_left",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(16 * 5, 0, 16, 16),
                        new Rectangle(16 * 6, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_right",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(16 * 7, 0, 16, 16),
                        new Rectangle(16 * 8, 0, 16, 16),
                    }
                },
            }));

            var negativeEffect = content.Load<Effect>("Shaders/Negative");

            _sprite.Effect = negativeEffect;
        }

        public override void OnSpawn()
        {
            _moveX = Input.NewAxisAction(Keys.A, Keys.D);
            _moveY = Input.NewAxisAction(Keys.W, Keys.S);
            _interact = Input.NewPressedAction(Keys.E);
        }

        public override void OnTick(GameTime gameTime)
        {
            var delta = gameTime.GetDeltaTime();
            var movementInput = new Vector2(_moveX.GetAxis(), _moveY.GetAxis());
            var length = movementInput.LengthSquared();

            if (length > 0f)
            {
                if (length > 1f) movementInput.Normalize();

                _direction = movementInput;

                // Should pause / unpause instead of changing animation data.
                // Ideally, we'd never dip into animation properties.
                _spriteSheet.CurrentAnimation.Loop = true;
            }
            else
            {
                _spriteSheet.CurrentAnimation.Loop = false;
            }

            _velocity += movementInput * ACCELERATION * delta;
            _velocity *= DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_SPEED, MAX_SPEED);
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -MAX_SPEED, MAX_SPEED);

            _body.Move(ref _velocity, EntityLayers.Enemy | EntityLayers.Wall, HandleCollision);

            Animate(_direction);

            if (_interact.WasPressed())
            {
                var fireball = new Fireball(_direction * 100f * delta);
                var fireballEntity = new Entity(Origin.Center, fireball);

                Level.Spawn(fireballEntity, _body.CalculateBounds().Center + _direction * 10f);
            }

            Level.Camera.Position = Vector2.SmoothStep(Level.Camera.Position, Entity.Position, 0.25f);
        }

        private Vector2 HandleCollision(Hit hit)
        {
            if (hit.Other.Entity.TryGetComponent<LevelTrigger>(out var transition))
            {
                App.Current.LoadOgmoLevel(transition.LevelName, transition.SpawnPoint);
            }

            if (hit.Other.IsSolid)
            {
                return hit.Slide();
            }

            return hit.Ignore();
        }

        private void Animate(Vector2 movementInput)
        {
            if (Math.Abs(movementInput.X) > Math.Abs(movementInput.Y))
            {
                if (movementInput.X < 0f)
                {
                    _spriteSheet.Play("walk_left");
                }
                else
                {
                    _spriteSheet.Play("walk_right");
                }
            }
            else
            {
                if (movementInput.Y < 0f)
                {
                    _spriteSheet.Play("walk_up");
                }
                else
                {
                    _spriteSheet.Play("walk_down");

                }
            }
        }
    }
}
