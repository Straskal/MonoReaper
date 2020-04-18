using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// A world object is any object that has a position is layout space.
    /// </summary>
    public sealed class WorldObject
    {
        internal WorldObject(Layout layout)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            Behaviors = new BehaviorList(this);
            Points = new WorldObjectPointList(this);
            Timers = new TimerList(Layout.Game);
            Tags = Array.Empty<string>();
            SpatialType = SpatialType.Overlap;
        }

        public Layout Layout { get; }
        public BehaviorList Behaviors { get; }
        public WorldObjectPointList Points { get; }
        public TimerList Timers { get; }
        public string[] Tags { get; set; }
        public bool IsMirrored { get; set; }
        public SpatialType SpatialType { get; set; }
        public int ZOrder { get; set; }
        public bool IsSolid => SpatialType.HasFlag(SpatialType.Solid);

        public Vector2 PreviousPosition { get; private set; }
        public WorldObjectBounds PreviousBounds { get; private set; }
        public SpatialType PreviousSpatialType { get; private set; }
        public bool IsDestroyed { get; private set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        private Point _origin;
        public Point Origin
        {
            get => _origin;
            set => _origin = value;
        }

        private WorldObjectBounds _bounds;
        public WorldObjectBounds Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public int Width
        {
            get => _bounds.Width;
            set => _bounds.Width = value;
        }

        public int Height
        {
            get => _bounds.Height;
            set => _bounds.Height = value;
        }

        public void SetX(float x)
        {
            _position.X = x;
        }

        public void SetY(float y)
        {
            _position.Y = y;
        }

        public void Move(float x, float y)
        {
            _position.X += x;
            _position.Y += y;
            UpdateBBox();
        }

        public void Move(Vector2 direction)
        {
            _position += direction;
            UpdateBBox();
        }

        /// <summary>
        /// Updates the world object's position in the layout's spatial grid. Must be called after directly modifying a world object's position.
        /// </summary>
        public void UpdateBBox()
        {
            if (IsDestroyed)
                return;

            InternalUpdateBBox();
            Layout.Grid.Update(this);
        }

        /// <summary>
        /// Destroys the world object.
        /// </summary>
        public void Destroy()
        {
            Layout.Objects.Destroy(this);
        }

        public void InternalUpdateBBox()
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
                throw new InvalidOperationException("Attempting to destroy world object that has already been destroyed.");

            IsDestroyed = true;
        }
    }
}
