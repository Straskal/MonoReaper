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

        public override bool Intersect(BoxCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            throw new NotImplementedException();
        }

        public override bool Intersect(CircleCollider collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            throw new NotImplementedException();
        }

        public override void Register()
        {
            foreach (var box in _colliders)
            {
                box.Register();
            }
        }

        public override void Unregister()
        {
            foreach (var box in _colliders)
            {
                box.Unregister();
            }
        }

        public override void UpdateBBox()
        {
            foreach (var box in _colliders)
            {
                box.UpdateBBox();
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

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            foreach (var box in _colliders) 
            {
                box.DebugDraw(renderer, gameTime);
            }
        }
    }
}
