using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class Door : Entity
    {
        private readonly EntityData data;
        private Sprite sprite;

        public Door(EntityData data)
        {
            this.data = data;
            Id = data.Fields.GetString("Id");
        }

        public string Id { get; set; }

        public override void Spawn()
        {
            GraphicsComponent = sprite = new Sprite(this, Store.Gfx.PressurePlate)
            {
                SourceRectangle = new Rectangle(0, 0, 32, 16),
                DrawOrder = 0
            };
            Collider = new BoxCollider(this, 32, 16);
            Collider.Enable();
            base.Spawn();
        }

        public void Open() 
        {
            GraphicsComponent = null;
            Collider.Disable();
        }

        public void Close() 
        {
        }
    }
}
