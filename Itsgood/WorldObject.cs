using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{ 
    public class WorldObject
    {
        private readonly List<Behavior> _behaviors;

        private Vector2 _positionRemainder;
        private Vector2 _position;
        private Point _origin;
        private Rectangle _bounds;

        internal WorldObject(Layout layout) 
        {
            Layout = layout;

            _behaviors = new List<Behavior>();
        }

        public Layout Layout { get; }
        public string ImageFilePath { get; set; }
        public Texture2D Image { get; set; }
        public string EffectFilePath { get; set; }
        public Effect Effect { get; set; }
        public bool IsEffectEnabled { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle PreviousBounds { get; private set; }
        public bool IsMirrored { get; set; }
        public bool IsSolid { get; set; }
        internal bool MarkedForDestroy { get; private set; }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
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

        public WorldObject MoveX(float amount)
        {
            _positionRemainder.X += amount;

            int pixelsToMove = (int)Math.Round(_positionRemainder.X);

            if (pixelsToMove != 0) 
            {
                _positionRemainder.X -= pixelsToMove;

                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.TestOverlapOffset(this, sign, 0, out var collision))
                        return collision;

                    _position.X += sign;
                    pixelsToMove -= sign;
                }
            }

            return null;
        }

        public WorldObject MoveY(float amount)
        {
            _positionRemainder.Y += amount;

            int pixelsToMove = (int)Math.Round(_positionRemainder.Y);

            if (pixelsToMove != 0)
            {
                _positionRemainder.Y -= pixelsToMove;

                int sign = Math.Sign(pixelsToMove);

                while (pixelsToMove != 0)
                {
                    if (Layout.TestOverlapOffset(this, 0, sign, out var collision))
                        return collision;

                    _position.Y += sign;
                    pixelsToMove -= sign;
                }
            }

            return null;
        }

        public void UpdateBBox() 
        {
            _bounds.X = (int)Math.Round(Position.X - Origin.X);
            _bounds.Y = (int)Math.Round(Position.Y - Origin.Y);

            Layout.UpdateGridCell(this);
        }

        public void Destroy() 
        {
            Layout.Destroy(this);
        }

        internal void AddBehavior<T>() where T : Behavior, new()
        {
            _behaviors.Add(new T { Owner = this });
        }

        internal void AddBehavior<T, U>(U state) where T : Behavior<U>, new()
        {
            _behaviors.Add(new T { Owner = this, State = state });
        }

        internal void Initialize() 
        {
            foreach (var behavior in _behaviors) 
            {
                behavior.Initialize();
            }
        }

        internal void OnCreated() 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerCreated();
            }
        }

        internal void OnDestroyed() 
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerDestroyed();
            }
        }

        internal void Load() 
        {
            var contentManager = Layout.Game.Content;

            if (!string.IsNullOrWhiteSpace(ImageFilePath))
                Image = contentManager.Load<Texture2D>(ImageFilePath);

            if (!string.IsNullOrWhiteSpace(EffectFilePath))
                Effect = contentManager.Load<Effect>(EffectFilePath);
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
