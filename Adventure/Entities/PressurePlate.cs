using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class PressurePlate : Entity
    {
        private readonly EntityData data;
        private Sprite sprite;
        private Door door;

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

        public override void Start()
        {
            door = World.FirstOrDefault<Door>(door => door.Id == data.Fields.GetString("DoorId"));

            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            var isDown = false;

            foreach (var entity in World.GetOverlappingEntities(Origin.Center.Tranform(Position.X, Position.Y, 16, 16))) 
            {
                if (entity is Barrel || entity is Player) 
                {
                    isDown = true;
                    break;
                }
            }

            if (isDown && !IsDown)
            {
                // Play trigger sound

                sprite.SourceRectangle = new Rectangle(16, 16, 16, 16);
                door?.Open();
            }
            else if (!isDown && IsDown) 
            {
                // Play release sound

                sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
                door?.Close();
            }

            IsDown = isDown;
        }
    }
}
