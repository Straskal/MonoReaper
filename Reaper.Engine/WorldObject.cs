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

        internal bool MarkedForDestroy { get; private set; }

        public SpatialType SpatialType
        {
            get => _type;
            set
            {
                _type = value;
                Layout.Grid.UpdateType(this);
            }
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

        public T GetBehavior<T>() where T : Behavior
        {
            return _behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        public bool TryGetBehavior<T>(out T behavior) where T : Behavior
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

        /// <summary>
        /// Updates the world object's position in the layout's spatial grid. Must be called after directly modifying a world object's position.
        /// </summary>
        public void UpdateBBox()
        {
            if (MarkedForDestroy)
                return;

            InternalUpdateBBox();
            Layout.Grid.Update(this);
        }

        /// <summary>
        /// Destroys the world object.
        /// </summary>
        public void Destroy()
        {
            Layout.Destroy(this);
        }

        public void InternalUpdateBBox()
        {
            _bounds.X = Position.X - Origin.X;
            _bounds.Y = Position.Y - Origin.Y;
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
            const float opacity = 0.3f;
            Color color;

            switch (SpatialType) 
            {
                case SpatialType.Pass:
                    color = Color.Pink * opacity;
                    break;
                case SpatialType.Overlap:
                    color = Color.Blue * opacity;
                    break;
                default:
                    color = Color.Red * opacity;
                    break;
            }

            renderer.DrawRectangle(Bounds.ToRectangle(), color);

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
            MarkedForDestroy = true;
        }
    }
}
