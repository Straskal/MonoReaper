using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine.Collision
{
    /// <summary>
    /// This component is used for tracking collidable objects.
    /// </summary>
    public class Box : Component
    {
        /// <summary>
        /// The partition cell points that contain this box.
        /// </summary>
        internal List<Point> PartitionCellPoints { get; } = new();

        public Box(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Box(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Box(float x, float y, float width, float height, int layerMask)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerMask = layerMask;
        }

        public event CollidedWithCallback CollidedWith;

        /// <summary>
        /// Gets or sets the box's X position relative to the entity.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the box's Y position relative to the entity.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the box's width.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the box's height.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the box's layer mask.
        /// </summary>
        /// <remarks>
        /// Layer masks can be used to identify properties of a box. Like solid objects, hazardous objects, interactable object, etc...
        /// </remarks>
        public int LayerMask { get; set; }

        public override void OnSpawn()
        {
            Level.Partition.Add(this);
        }

        public override void OnDestroy()
        {
            Level.Partition.Remove(this);
        }

        /// <summary>
        /// Calculates and returns the Box's bounds relative to the entity's origin and position.
        /// </summary>
        /// <returns></returns>
        public RectangleF CalculateBounds()
        {
            return Entity.Origin.Tranform(Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);
        }

        /// <summary>
        /// Move the entity in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction)
        {
            Entity.Position += direction;
            UpdateBBox();
        }

        /// <summary>
        /// Sets the entity's position.
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Vector2 position)
        {
            Entity.Position = Entity.Origin.Invert(position.X, position.Y, Width, Height).Position;
            UpdateBBox();
        }

        /// <summary>
        /// Updates the Box in the level's spatial partition.
        /// </summary>
        /// <remarks>
        /// This method must be called after updating the entity's position.
        /// </remarks>
        public void UpdateBBox()
        {
            Level.Partition.Update(this);
        }

        public override void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(CalculateBounds().ToXnaRect(), Color.White);
        }

        internal void NotifyCollidedWith(Body body, Collision collision) 
        {
            CollidedWith?.Invoke(body, collision);
        }
    }
}
