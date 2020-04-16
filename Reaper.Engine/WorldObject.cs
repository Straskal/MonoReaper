using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// A world object is any object that has a position is layout space.
    /// </summary>
    public sealed class WorldObject
    {
        // Should probably break the timer out into another class. Singletons could benefit from this as well.
        private struct Timer
        {
            public string Name;
            public float Time;
            public Action TimerCallback;
        }

        private readonly Dictionary<string, WorldObjectPoint> _points = new Dictionary<string, WorldObjectPoint>();
        private readonly List<Timer> _timers = new List<Timer>();
        private readonly List<Behavior> _behaviors;

        private SpatialType _type = SpatialType.Overlap;
        private Vector2 _position = Vector2.Zero;
        private Point _origin = Point.Zero;
        private WorldObjectBounds _bounds = WorldObjectBounds.Empty;

        internal WorldObject(Layout layout)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));

            _behaviors = new List<Behavior>();
        }

        public string[] Tags { get; set; } = new string[0];
        public bool IsMirrored { get; set; }
        public bool IsSolid => SpatialType.HasFlag(SpatialType.Solid);
        public int ZOrder { get; set; }
        public Layout Layout { get; }

        public Vector2 PreviousPosition { get; private set; }
        public WorldObjectBounds PreviousBounds { get; private set; }
        public SpatialType PreviousSpatialType { get; private set; }
        public bool IsDestroyed { get; private set; }

        public SpatialType SpatialType
        {
            get => _type;
            set => _type = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Width
        {
            get => _bounds.Width;
            set => _bounds.Width = value;
        }

        public float Height
        {
            get => _bounds.Height;
            set => _bounds.Height = value;
        }

        public Point Origin
        {
            get => _origin;
            set => _origin = value;
        }

        public WorldObjectBounds Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public void AddPoint(string name, float x, float y)
        {
            _points.Add(name, new WorldObjectPoint(this, x, y));
        }

        public WorldObjectPoint GetPoint(string name)
        {
            if (!_points.TryGetValue(name, out var point))
                throw new ArgumentException($"World object does not have point {name}");

            return point;
        }

        public T GetBehavior<T>() where T : class
        {
            return _behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        public bool TryGetBehavior<T>(out T behavior) where T : class
        {
            behavior = _behaviors.FirstOrDefault(b => b is T) as T;
            return behavior != null;
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

        public void StartTimer(string name, float time, Action timerCallback)
        {
            _timers.Add(new Timer { Name = name, Time = Layout.Game.TotalTime + time, TimerCallback = timerCallback });
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

        public void AddBounds(float x, float y, int width, int height)
        {
            Layout.Grid.Add(this, new WorldObjectBounds(x, y, width, height));
        }

        internal void AddBehavior(Func<WorldObject, Behavior> createFunc)
        {
            _behaviors.Add(createFunc?.Invoke(this));
        }

        internal void Load(ContentManager contentManager)
        {
            foreach (var behavior in _behaviors)
                behavior.Load(contentManager);
        }

        internal void OnCreated()
        {
            PreviousPosition = Position;
            PreviousBounds = Bounds;
            PreviousSpatialType = SpatialType;

            foreach (var behavior in _behaviors)
                behavior.OnOwnerCreated();
        }

        internal void OnLayoutStarted()
        {
            foreach (var behavior in _behaviors)
                behavior.OnLayoutStarted();
        }

        internal void Tick(GameTime gameTime)
        {
            TickTimers((float)gameTime.TotalGameTime.TotalSeconds);

            foreach (var behavior in _behaviors)
                behavior.Tick(gameTime);
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var behavior in _behaviors)
                behavior.PostTick(gameTime);
        }

        internal void Draw(Renderer renderer)
        {
            foreach (var behavior in _behaviors)
                behavior.Draw(renderer);
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var behavior in _behaviors)
                behavior.DebugDraw(renderer);
        }

        internal void OnDestroyed()
        {
            foreach (var behavior in _behaviors)
                behavior.OnOwnerDestroyed();
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
            IsDestroyed = true;
        }

        private void TickTimers(float time)
        {
            _timers.RemoveAll(timer => 
            {
                if (time > timer.Time)
                {
                    timer.TimerCallback?.Invoke();
                    return true;
                }
                return false;
            });
        }
    }
}
