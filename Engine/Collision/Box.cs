﻿using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    public class Box : Collider
    {
        public Box(Entity entity)
            : this(entity, 0f, 0f)
        {
        }

        public Box(Entity entity, float width, float height)
            : this(entity, width, height, 0)
        {
        }

        public Box(Entity entity, float width, float height, int layerMask) 
            : this(entity, 0f, 0f, width, height, layerMask)
        {
        }

        public Box(Entity entity, float x, float y, float width, float height)
            : this(entity, x, y, width, height, 0)
        {
        }

        public Box(Entity entity, float x, float y, float width, float height, int layerMask)
            : base(entity)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerMask = layerMask;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        public override RectangleF Bounds 
        {
            get => Entity.Origin.Tranform(Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);
        }

        public override bool Intersect(Box collider, IntersectionPath path, out float time, out Vector2 contact, out Vector2 normal)
        {
            return Intersection.MovingRectangleVsRectangle(Bounds, path, collider.Bounds, out time, out contact, out normal);
        }

        public override bool Intersect(CircleCollider collider, IntersectionPath velocity, out float time, out Vector2 contact, out Vector2 normal)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.White);
        }
    }
}
