using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using static Adventure.Components.Tilemap;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class TilemapCollider : Collider
    {
        private readonly List<BoxCollider> _colliders = new();

        public TilemapCollider(Entity entity, int width, int height, MapData mapData)
            : this(entity, width, height, 0, mapData)
        {
        }

        public TilemapCollider(Entity entity, int width, int height, int layerMask, MapData mapData) 
            : base(entity)
        {
            Width = width;
            Height = height;
            Layer = layerMask;

            foreach (var tile in mapData.Tiles)
            {
                _colliders.Add(new BoxCollider(entity, tile.Position.X, tile.Position.Y, mapData.CellSize, mapData.CellSize, EntityLayers.Solid));
            }
        }

        public int Width 
        {
            get;
        }

        public int Height 
        {
            get;
        }

        public override RectangleF Bounds 
        {
            get => new(0, 0, Width, Height);
        }

        public override void Enable()
        {
            foreach (var box in _colliders)
            {
                box.Enable();
            }
        }

        public override void Disable()
        {
            foreach (var box in _colliders)
            {
                box.Disable();
            }
        }

        public override void UpdateBounds()
        {
            foreach (var box in _colliders)
            {
                box.UpdateBounds();
            }
        }

        public override void Move(Vector2 direction)
        {
            throw new NotImplementedException();
        }

        public override void SetPosition(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            foreach (var box in _colliders) 
            {
                box.Draw(renderer, gameTime);
            }
        }

        public override bool IsOverlapped(BoxCollider collider)
        {
            throw new NotImplementedException();
        }

        public override bool IsOverlapped(CircleCollider collider)
        {
            throw new NotImplementedException();
        }

        public override bool Overlaps(Collider collider)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(Collider collider, Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }

        public override bool IsIntersected(BoxCollider collider, Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }

        public override bool IsIntersected(CircleCollider collider, Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }
    }
}
