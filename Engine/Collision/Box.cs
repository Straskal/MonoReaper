using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// This component is used for tracking collidable objects.
    /// </summary>
    /// <remarks>
    /// When a box component is attached to an entity, the entity's position should not be directly updated.
    /// Instead, Box methods should be used to move an entity because they also update tracking in the level's spatial partition.
    /// </remarks>
    public class Box : Component
    {
        /// <summary>
        /// The partition cell points that contain this box.
        /// These are cached on the box itself to avoid unnecessary lookups in the partition.
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
            return Offset.GetRect(Entity.Origin, Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);
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
            Entity.Position = Offset.Create(Entity.Origin, position.X, position.Y, Width, Height);
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
    }
}
