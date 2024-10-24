using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure.Core
{
    public sealed class Player : Actor
    {
        public const float MovementSpeed = 1000f;
        public const float MovementSpeedMax = 0.75f;

        public Sprite Sprite { get; private set; }

        public override void Spawn()
        {
            Sprite = new Sprite(ContentCache.Gfx.Player);
            Sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
            ShapeLocal = CollisionShape.CreateCircle(0, 0, 8);
            Game.EnableUpdate(this);
            Game.EnableCollision(this);
        }

        public override void Update(float deltaTime) 
        {
            var movementInput = Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);
            if (movementInput.LengthSquared() > 1f)
            {
                movementInput.Normalize();
            }
            var velocity = movementInput * MovementSpeed * deltaTime;
            velocity.X = MathHelper.Clamp(velocity.X, -MovementSpeedMax, MovementSpeedMax);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MovementSpeedMax, MovementSpeedMax);
            SlideMove(velocity);
        }

        public override void Draw(float deltaTime)
        {
            Game.Renderer.Draw(Sprite, Position);
        }
    }
}
