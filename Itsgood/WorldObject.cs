using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{ 
    public class WorldObject
    {
        private Vector2 _positionRemainder;
        private Vector2 _position;
        private Point _origin;
        private Rectangle _bounds;

        internal WorldObject(Layout layout) 
        {
            Layout = layout;
        }

        public Layout Layout { get; }
        public string ImageFilePath { get; set; }
        public Texture2D Image { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle PreviousBounds { get; private set; }
        public bool IsMirrored { get; set; }
        public bool IsSolid { get; set; }
        internal List<Behavior> Behaviors { get; } = new List<Behavior>();
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
            return Behaviors.FirstOrDefault(behavior => behavior is T) as T;
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
                    if (Layout.Grid.TestOverlapOffset(this, sign, 0, out var collision))
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
                    if (Layout.Grid.TestOverlapOffset(this, 0, sign, out var collision))
                        return collision;

                    _position.Y += sign;
                    pixelsToMove -= sign;
                }
            }

            return null;
        }

        public void UpdateBBox() 
        {
            UpdateBBoxNoGrid();

            Layout.Grid.Update(this);
        }

        public void UpdateBBoxNoGrid() 
        {
            _bounds.X = (int)Math.Round(Position.X - Origin.X);
            _bounds.Y = (int)Math.Round(Position.Y - Origin.Y);
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
