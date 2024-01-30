using Engine;
using System.Collections.Generic;
using static Adventure.Constants;
using static Adventure.Entities.Tilemap;

namespace Adventure.Components
{
    public sealed class TilemapCollider
    {
        private readonly List<CollisionComponent> colliders = new();

        public TilemapCollider(Entity entity, MapData mapData)
        {
            SetUpTileColliders(entity, mapData);
        }

        public void Enable()
        {
            foreach (var collider in colliders)
            {
                collider.Enable();
            }
        }

        public void Disable()
        {
            foreach (var collider in colliders)
            {
                collider.Disable();
            }
        }

        private void SetUpTileColliders(Entity entity, MapData mapData)
        {
            foreach (var tile in mapData.Tiles)
            {
                var collider = CollisionComponent.CreateBox(entity, tile.Position.X, tile.Position.Y, mapData.CellSize, mapData.CellSize);
                collider.Layer = EntityLayers.Solid;
                colliders.Add(collider);
            }
        }
    }
}
