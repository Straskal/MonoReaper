using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class Door : Entity
    {
        private Sprite sprite;

        public Door(EntityData data)
        {
            Tags.Add(data.Fields.GetString("Id"));
        }

        public string Id { get; set; }

        public override void Spawn()
        {
            GraphicsComponent = sprite = new Sprite(this, Store.Gfx.PressurePlate)
            {
                SourceRectangle = new Rectangle(0, 0, 32, 16),
                DrawOrder = 0
            };
            Collider = CollisionComponent.CreateBox(this, 0f, 0f, 32, 16);
            Collider.Layer = EntityLayers.Solid;
            Collider.Enable();
        }

        public void Open()
        {
            GraphicsComponent = null;
            Collider.Disable();
        }

        public void Close()
        {
            GraphicsComponent = sprite;
            Collider.Enable();
        }
    }
}
