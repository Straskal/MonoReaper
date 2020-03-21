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

        private Vector2 _positionRemainder;
        private Vector2 _position;
        private Point _origin;
        private Rectangle _bounds;

        internal WorldObject(Layout layout) 
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));

            _behaviors = new List<Behavior>();
        }

        public Layout Layout { get; }
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle PreviousBounds { get; private set; }
        public bool IsMirrored { get; set; }
        public bool IsSolid { get; set; }
        public int ZOrder { get; set; }

        internal bool MarkedForDestroy { get; private set; }

        /// <summary>
        /// The world object's position. UpdateBBox() must be called after modifying this property.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 DrawPosition
        {
            get => _position + _positionRemainder;
        }

        public Point Origin 
        {
            get => _origin;
            set => _origin = value;
        }

        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public T GetBehavior<T>() where T : Behavior 
        {
            return _behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        public IEnumerable<Behavior> GetBehaviors() 
        {
            return _behaviors;
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

            _positionRemainder.X += amount;

            int pixelsToMove = (int)Math.Round(_positionRemainder.X);

            if (pixelsToMove != 0) 
            {
                _positionRemainder.X -= pixelsToMove;

                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.TestOverlapSolidOffset(this, sign, 0, out var collision))
                    {
                        _positionRemainder.X = 0f;

                        worldObject = collision;
                        return true;
                    }

                    _position.X += sign;
                    pixelsToMove -= sign;
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

            _positionRemainder.Y += amount;

            int pixelsToMove = (int)Math.Round(_positionRemainder.Y);

            if (pixelsToMove != 0)
            {
                _positionRemainder.Y -= pixelsToMove;

                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.TestOverlapSolidOffset(this, 0, sign, out var collision))
                    {
                        _positionRemainder.Y = 0f;

                        worldObject = collision;
                        return true;
                    }

                    _position.Y += sign;
                    pixelsToMove -= sign;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the world object's position in the layout's spatial grid.
        /// </summary>
        public void UpdateBBox() 
        {
            _bounds.X = (int)Math.Round(Position.X - Origin.X);
            _bounds.Y = (int)Math.Round(Position.Y - Origin.Y);

            Layout.UpdateGridCell(this);
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
            _behaviors.Add(createFunc?.Invoke(this));
        }

        internal void Load(ContentManager contentManager) 
        {
            foreach (var behavior in _behaviors) 
            {
                behavior.Load(contentManager);
            }
        }

        internal void OnCreated() 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerCreated();
            }
        }

        internal void OnLayoutStarted() 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnLayoutStarted();
            }
        }

        internal void Tick(GameTime gameTime) 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Tick(gameTime);
            }
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.PostTick(gameTime);
            }
        }

        internal void Draw(LayoutView view)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Draw(view);
            }
        }

        internal void OnDestroyed() 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerDestroyed();
            }
        }

        internal void UpdatePreviousPosition()
        {
            PreviousPosition = Position;
            PreviousBounds = Bounds;
        }

        internal void MarkForDestroy() 
        {
            MarkedForDestroy = true;
        }
    }
}
