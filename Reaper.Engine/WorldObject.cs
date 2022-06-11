using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// A world object is any object that has a position is layout space.
    /// </summary>
    public sealed class WorldObject
    {
        public WorldObject(Layout layout)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            Behaviors = new BehaviorList(this);
            Points = new WorldObjectPointList(this);
            Timers = new TimerList(Layout.Game);
            Tags = Array.Empty<string>();
            SpatialType = SpatialType.Overlap;
        }

        /// <summary>
        /// The layout that this world object lives.
        /// </summary>
        public Layout Layout { get; }

        /// <summary>
        /// Collection of behaviors owned by this world object.
        /// </summary>
        public BehaviorList Behaviors { get; }

        /// <summary>
        /// A collection of points on this world object.
        /// </summary>
        public WorldObjectPointList Points { get; }

        /// <summary>
        /// This world objects timer list. A timers lifespan is dependent on it's owner.
        /// </summary>
        public TimerList Timers { get; }

        /// <summary>
        /// Any tags associated with this world object.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// True if the object is mirrored.
        /// 
        /// TODO: Use bit flags for a mirror mask that supports horizontal and vertical.
        /// </summary>
        public bool IsMirrored { get; set; }

        /// <summary>
        /// The object's physical attribute.
        /// </summary>
        public SpatialType SpatialType { get; set; }

        /// <summary>
        /// The Z order layer that this object will be drawn to.
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// True if the object is solid.
        /// </summary>
        public bool IsSolid => SpatialType.HasFlag(SpatialType.Solid);

        // Previous tracking is kind of annoying and only exists because some other areas need to be reimplemented.
        public Vector2 PreviousPosition { get; private set; }
        public WorldObjectBounds PreviousBounds { get; private set; }
        public SpatialType PreviousSpatialType { get; private set; }

        /// <summary>
        /// True if the object has been marked for destruction next frame.
        /// </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary>
        /// The object's position in world space.
        /// </summary>
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// The objects origin.
        /// </summary>
        private Point _origin;
        public Point Origin
        {
            get => _origin;
            set => _origin = value;
        }

        /// <summary>
        /// The bounds, or size of the object in pixels.
        /// </summary>
        private WorldObjectBounds _bounds;
        public WorldObjectBounds Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        /// <summary>
        /// The object's width in pixels.
        /// </summary>
        public int Width
        {
            get => _bounds.Width;
            set => _bounds.Width = value;
        }

        /// <summary>
        /// The object's height in pixels.
        /// </summary>
        public int Height
        {
            get => _bounds.Height;
            set => _bounds.Height = value;
        }

        // Need to set BBOX below?
        public void SetX(float x)
        {
            _position.X = x;
        }

        public void SetY(float y)
        {
            _position.Y = y;
        }

        /// <summary>
        /// Moves the object in the given direction.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(float x, float y)
        {
            _position.X += x;
            _position.Y += y;

            UpdateBBox();
        }

        /// <summary>
        /// Moves the object in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction)
        {
            _position += direction;

            UpdateBBox();
        }

        /// <summary>
        /// Updates the world object's position in the layout's spatial grid. Must be called after directly modifying a world object's position.
        /// 
        /// TODO: Look into this. Not sure this approach is best.
        /// </summary>
        public void UpdateBBox()
        {
            if (IsDestroyed)
                return;

            InternalUpdateBBox();

            Layout.Grid.Update(this);
        }

        /// <summary>
        /// Marks the object for destruction next frame.
        /// </summary>
        public void Destroy()
        {
            Layout.Objects.Destroy(this);
        }

        internal void InternalUpdateBBox()
        {
            _bounds.X = Position.X - Origin.X;
            _bounds.Y = Position.Y - Origin.Y;
        }

        internal void Load()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Load();
            }
        }

        internal void OnCreated()
        {
            PreviousPosition = Position;
            PreviousBounds = Bounds;
            PreviousSpatialType = SpatialType;

            foreach (var behavior in Behaviors)
            {
                behavior.OnOwnerCreated();
            }
        }

        internal void OnLayoutStarted()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.OnLayoutStarted();
            }
        }

        internal void Tick(GameTime gameTime)
        {
            Timers.Tick();

            foreach (var behavior in Behaviors)
            {
                behavior.Tick(gameTime);
            }
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.PostTick(gameTime);
            }
        }

        internal void Draw(Renderer renderer)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Draw(renderer);
            }
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.DebugDraw(renderer);
            }
        }

        internal void OnDestroyed()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.OnOwnerDestroyed();
            }
        }

        internal void UpdatePreviousFrameData()
        {
            PreviousPosition = Position;
            PreviousBounds = Bounds;
            PreviousSpatialType = SpatialType;
        }

        /// <summary>
        /// Marks the object to be destroyed. Once this is set, it cannot be unset.
        /// </summary>
        internal void MarkForDestroy()
        {
            if (IsDestroyed)
            {
                throw new InvalidOperationException("Attempting to destroy world object that has already been destroyed.");
            }

            IsDestroyed = true;
        }
    }
}
