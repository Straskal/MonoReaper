using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class PressurePlate : Entity
    {
        private readonly EntityData data;
        private Sprite sprite;

        public PressurePlate(EntityData data) 
        {
            this.data = data;
        }

        public bool IsDown { get; private set; }

        public override void Spawn()
        {
            GraphicsComponent = sprite = new Sprite(this, Store.Gfx.PressurePlate)
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16),
                DrawOrder = 0
            };

            base.Spawn();
        }

        public override void Update(GameTime gameTime)
        {
            var isDown = false;

            foreach (var entity in World.GetOverlappingEntities(new RectangleF(Position.X - 8, Position.Y - 8, 16, 16))) 
            {
                if (entity is Barrel || entity is Player) 
                {
                    isDown = true;
                    break;
                }
            }

            if (isDown && !IsDown)
            {
                var door = World.FindWithTag<Door>(data.Fields.GetString("DoorId"));
                // Play trigger sound

                sprite.SourceRectangle = new Rectangle(16, 16, 16, 16);
                door?.Open();
            }
            else if (!isDown && IsDown) 
            {
                var door = World.FindWithTag<Door>(data.Fields.GetString("DoorId"));
                // Play release sound

                sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
                door?.Close();
            }

            IsDown = isDown;
        }
    }
}
