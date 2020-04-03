using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// A world object is any object that has a position is layout space.
    /// </summary>
    public sealed class WorldObject
    {
        private Vector2 _position = Vector2.Zero;
        private Rectangle _bounds = Rectangle.Empty;

        internal WorldObject(WorldObjectType woType, Layout layout)
        {
            Type = woType ?? throw new ArgumentNullException(nameof(woType));
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));

            InitializeBehaviors();
        }

        public string Name => Type.Name;
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle PreviousBounds { get; private set; }
        public bool IsMirrored { get; set; }
        public bool IsSolid => SpatialType.HasFlag(SpatialType.Solid);
        public int ZOrder { get; set; }
        public Layout Layout { get; }

        internal WorldObjectType Type { get; }
        internal bool MarkedForDestroy { get; private set; }

        public SpatialType SpatialType
        {
            get => Type.SpatialType;
            set
            {
                var previous = Type.SpatialType;
                Type.SpatialType = value;

                Layout.Grid.UpdateType(this, previous);
            }
        }

        /// <summary>
        /// The world object's pixel perfect position. UpdateBBox() must be called after modifying this property.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public int Width
        {
            get => Type.Width;
            set => Type.Width = value;
        }

        public int Height
        {
            get => Type.Height;
            set => Type.Height = value;
        }

        public Point Origin
        {
            get => Type.Origin;
            set => Type.Origin = value;
        }

        public Rectangle Bounds 
        {
            get => _bounds;
            set => _bounds = value;
        }

        public void AddBehavior<T>() where T : Behavior, new() 
        {
            AddBehavior(typeof(T));
        }

        public void AddBehavior(Type type)
        {
            var behavior = (Behavior)Activator.CreateInstance(type);
            //behavior.Load(Layout.Content);
            behavior.Owner = this;
            behavior.OnOwnerCreated();
            behavior.OnLayoutStarted();

            Type.Behaviors.Add(behavior);
        }

        public T GetBehavior<T>() where T : Behavior
        {
            return Type.Behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        public bool TryGetBehavior<T>(out T behavior) where T : Behavior
        {
            behavior = Type.Behaviors.FirstOrDefault(b => b is T) as T;
            return behavior != null;
        }

        /// <summary>
        /// Move the object on the x axis and perform stepped collision checks at pixel perfect positions.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public bool MoveXAndCollide(float amount, out WorldObject worldObject)
        {
            worldObject = null;

            int pixelsToMove = (int)Math.Round(amount);

            if (pixelsToMove != 0)
            {
                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.Grid.TestSolidOverlapOffset(this, sign, 0, out var collision))
                    {
                        worldObject = collision;
                        return true;
                    }

                    _position.X += sign;
                    pixelsToMove -= sign;

                    UpdateBBox();
                }
            }

            return false;
        }

        /// <summary>
        /// Move the object on the y axis and perform stepped collision checks at pixel perfect positions.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public bool MoveYAndCollide(float amount, out WorldObject worldObject)
        {
            worldObject = null;

            int pixelsToMove = (int)Math.Round(amount);

            if (pixelsToMove != 0)
            {
                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.Grid.TestSolidOverlapOffset(this, 0, sign, out var collision))
                    {
                        worldObject = collision;
                        return true;
                    }

                    _position.Y += sign;
                    pixelsToMove -= sign;

                    UpdateBBox();
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the world object's position in the layout's spatial grid. Must be called after directly modifying a world object's position.
        /// </summary>
        public void UpdateBBox()
        {
            _bounds.X = (int)Math.Round(Position.X - Origin.X);
            _bounds.Y = (int)Math.Round(Position.Y - Origin.Y);
            _bounds.Width = Width;
            _bounds.Height = Height;

            Layout.Grid.Update(this);
        }

        /// <summary>
        /// Destroys the world object.
        /// </summary>
        public void Destroy()
        {
            Layout.Destroy(this);
        }

        internal void AddBehavior(Func<WorldObject, Behavior> createFunc)
        {
            Type.Behaviors.Add(createFunc?.Invoke(this));
        }

        internal void Load(ContentManager contentManager)
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.Load(contentManager);
            }
        }

        internal void OnCreated()
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.OnOwnerCreated();
            }
        }

        internal void OnLayoutStarted()
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.OnLayoutStarted();
            }
        }

        internal void Tick(GameTime gameTime)
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.Tick(gameTime);
            }
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.PostTick(gameTime);
            }
        }

        internal void Draw(LayoutView view)
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.Draw(view);
            }
        }

        internal void DebugDraw(LayoutView view)
        {
            var destination = new Rectangle(
              (int)(Position.X - Origin.X),
              (int)(Position.Y - Origin.Y),
              Bounds.Width,
              Bounds.Height);

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

            view.DrawRectangle(destination, color);

            foreach (var behavior in Type.Behaviors)
            {
                behavior.DebugDraw(view);
            }
        }

        internal void OnDestroyed()
        {
            foreach (var behavior in Type.Behaviors)
            {
                behavior.OnOwnerDestroyed();
            }
        }

        internal void UpdatePreviousPosition()
        {
            PreviousPosition = Position;
            PreviousBounds = Bounds;
        }

        /// <summary>
        /// Marks the object to be destroyed. Once this is set, it cannot be unset.
        /// </summary>
        internal void MarkForDestroy()
        {
            MarkedForDestroy = true;
        }

        private void InitializeBehaviors() 
        {
            foreach (var behavior in Type.Behaviors) 
            {
                behavior.Owner = this;
            }
        }
    }
}
