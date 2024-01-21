using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static Adventure.Entities.Tilemap;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class TilemapCollider : Collider
    {
        private readonly List<BoxCollider> colliders = new();

        public TilemapCollider(Entity entity, int width, int height, MapData mapData)
            : this(entity, width, height, 0, mapData)
        {
        }

        public TilemapCollider(Entity entity, int width, int height, uint layerMask, MapData data)
            : base(entity)
        {
            Width = width;
            Height = height;
            Layer = layerMask;

            SetUpTileColliders(data);
        }

        public int Width { get; }
        public int Height { get; }

        public override void Enable()
        {
            foreach (var box in colliders)
            {
                box.Enable();
            }
        }

        public override void Disable()
        {
            foreach (var box in colliders)
            {
                box.Disable();
            }
        }

        public override void Update()
        {
            foreach (var box in colliders)
            {
                box.Update();
            }
        }

        public override bool OverlapCircle(CircleF circle)
        {
            throw new NotImplementedException();
        }

        public override bool OverlapRectangle(RectangleF rectangle)
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

        public override void Draw(Renderer renderer)
        {
            foreach (var box in colliders)
            {
                box.Draw(renderer);
            }
        }

        private void SetUpTileColliders(MapData mapData)
        {
            foreach (var tile in mapData.Tiles)
            {
                colliders.Add(new BoxCollider(Entity, tile.Position.X, tile.Position.Y, mapData.CellSize, mapData.CellSize) 
                {
                    Layer = EntityLayers.Solid
                });
            }
        }

        public override bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            throw new NotImplementedException();
        }

        public override bool OverlapPoint(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public override RectangleF CalculateBounds()
        {
            throw new NotImplementedException();
        }
    }
}
