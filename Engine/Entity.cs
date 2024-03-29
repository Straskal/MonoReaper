﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public class Entity
    {
        public World World { get; internal set; }
        public HashSet<string> Tags { get; } = new();
        public Origin EntityOrigin { get; set; } = Origin.Center;
        public Vector2 Position { get; set; }
        public Collider Collider { get; protected set; }
        public GraphicsComponent GraphicsComponent { get; protected set; }
        public bool IsDestroyed { get; internal set; }

        public int DrawOrder
        {
            get
            {
                if (GraphicsComponent != null)
                {
                    return GraphicsComponent.DrawOrder;
                }
                return 0;
            }
        }

        public virtual void Spawn()
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            GraphicsComponent?.PostUpdate(gameTime);
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
            GraphicsComponent?.Draw(renderer, gameTime);
        }

        public virtual void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            Collider?.Draw(renderer, gameTime);
        }

        public RectangleF TransformOrigin(float width, float height)
        {
            return TransformOrigin(0f, 0f, width, height);
        }

        public RectangleF TransformOrigin(float xOffset, float yOffset, float width, float height)
        {
            // Default top left
            var result = new RectangleF(Position.X + xOffset, Position.Y + yOffset, width, height);

            switch (EntityOrigin)
            {
                case Origin.Center:
                    result.X -= result.Width * 0.5f;
                    result.Y -= result.Height * 0.5f;
                    break;
            }

            return result;
        }
    }
}
