using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class PressurePlate : Entity
    {
        private readonly string doorId;
        private Sprite sprite;
        private Door door;

        public PressurePlate(EntityData data) 
        {
            this.doorId = data.Fields.GetString("DoorId");
        }

        public bool IsPressed { get; private set; }

        public override void Spawn()
        {
            GraphicsComponent = sprite = new Sprite(this, Store.Gfx.PressurePlate)
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16),
                DrawOrder = 0
            };
        }

        public override void Update(GameTime gameTime)
        {
            var wasPressed = IsPressed;

            CheckIsPressed();

            if (wasPressed != IsPressed)
            {
                if (!wasPressed && IsPressed)
                {
                    sprite.SourceRectangle = new Rectangle(16, 16, 16, 16);
                    FindDoor()?.Open();
                }
                else if (wasPressed && !IsPressed)
                {
                    sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
                    FindDoor()?.Close();
                }
            }
        }

        private void CheckIsPressed()
        {
            IsPressed = false;

            // It would be cool to NOT check every frame, but instead have entities notify the pressure plate when they move.
            foreach (var collider in World.OverlapRectangle(new RectangleF(Position.X - 8, Position.Y - 8, 16, 16)))
            {
                if (collider.Entity is Barrel || collider.Entity is Player)
                {
                    IsPressed = true;
                    break;
                }
            }
        }

        private Door FindDoor() 
        {
            door ??= World.FindWithTag<Door>(doorId);
            return door;
        }
    }
}
