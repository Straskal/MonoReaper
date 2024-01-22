using Adventure.Content;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class PressurePlate : Trigger
    {
        private readonly string doorId;
        private readonly List<Entity> pressingEntities = new();
        private Sprite sprite;
        private Door door;

        public PressurePlate(EntityData data)
        {
            this.doorId = data.Fields.GetString("DoorId");
        }

        public bool IsPressed { get; private set; }

        public override void Spawn()
        {
            Collider = new BoxCollider(this, 16, 16);
            Collider.Layer = EntityLayers.Trigger;
            Collider.Enable();
            GraphicsComponent = sprite = new Sprite(this, Store.Gfx.PressurePlate)
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16),
                DrawOrder = 0
            };
        }

        public override void Update(GameTime gameTime)
        {
            var wasPressed = IsPressed;

            for (int i = 0; i < pressingEntities.Count; i++)
            {
                if (!(pressingEntities[i].IsAlive && pressingEntities[i].Collider.Overlaps(Collider)))
                {
                    pressingEntities.RemoveAt(i);
                    i--;
                }
            }

            IsPressed = pressingEntities.Count > 0;

            if (!wasPressed && IsPressed)
            {
                OpenDoor();
            }
            else if (wasPressed && !IsPressed)
            {
                CloseDoor();
            }
        }

        public override void OnTouch(Entity entity)
        {
            if (entity is Barrel || entity is Player && !pressingEntities.Contains(entity))
            {
                pressingEntities.Add(entity);
            }
        }

        private void OpenDoor()
        {
            door ??= World.FindWithTag<Door>(doorId);
            door?.Open();
            sprite.SourceRectangle = new Rectangle(16, 16, 16, 16);
        }

        private void CloseDoor()
        {
            door ??= World.FindWithTag<Door>(doorId);
            door?.Close();
            sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
        }
    }
}
