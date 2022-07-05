using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public class Box : Component
    {
        // Cached spatial partition data
        internal readonly int[] PartitionCells = new int[Partition.MAX_BOUNDS_BUCKETS];
        internal int PartitionCellCount = 0;

        public Box(float width, float height, bool isSolid) : this(width, height, isSolid, 0)
        {
        }

        public Box(float width, float height, bool isSolid, int layerMask)
        {
            Width = width;
            Height = height;
            IsSolid = isSolid;
            LayerMask = layerMask;
        }

        public bool IsSolid { get; set; }
        public int LayerMask { get; set; }

        public Vector2 Size { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override void OnSpawn()
        {
            Level.Partition.Add(this);
        }

        public override void OnDestroy()
        {
            Level.Partition.Remove(this);
        }

        public RectangleF CalculateBounds() 
        {
            return Offset.GetRect(
                Entity.Origin,
                Entity.Position.X,
                Entity.Position.Y,
                Width,
                Height);
        }

        public void MoveTo(Vector2 position) 
        {
            Entity.Position = position;

            UpdateBBox();
        }

        public void UpdateBBox()
        {
            if (Entity.IsDestroyed)
                return;

            Level.Partition.Update(this);
        }
    }
}
