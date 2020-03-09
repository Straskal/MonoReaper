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

        public WorldObject(Layout layout) 
        {
            Layout = layout;
        }

        public Layout Layout { get; }
        public string ImageFilePath { get; set; }
        public Texture2D Image { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public bool IsMirrored { get; set; }
        public bool IsSolid { get; set; }

        public Rectangle Bounds { get; private set; }
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle PreviousBounds { get; private set; }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }


        internal List<Behavior> Behaviors { get; } = new List<Behavior>();
        internal bool MarkedForDestroy { get; private set; }

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
                    var collision = Layout.Grid.TestOverlap(this, new Vector2(sign, 0));

                    if (collision != null)
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
                    var collision = Layout.Grid.TestOverlap(this, new Vector2(0, sign));

                    if (collision != null)
                        return collision;

                    _position.Y += sign;
                    pixelsToMove -= sign;
                }
            }

            return null;
        }

        public void UpdateBBox() 
        {
            Bounds = new Rectangle(
                (int)Math.Round(Position.X - Source.Width * 0.5f),
                (int)Math.Round(Position.Y - Source.Height * 0.5f),
                Source.Width, 
                Source.Height);

            Layout.Grid.Update(this);
        }

        public void UpdateBBoxNoGrid() 
        {
            Bounds = new Rectangle(
                (int)Math.Round(Position.X - Source.Width * 0.5f),
                (int)Math.Round(Position.Y - Source.Height * 0.5f),
                Source.Width,
                Source.Height);
        }

        internal void UpdatePreviousPosition()
        {
            PreviousPosition = Position;
            PreviousBounds = new Rectangle(
                (int)Math.Round(PreviousPosition.X - Source.Width * 0.5f),
                (int)Math.Round(PreviousPosition.Y - Source.Height * 0.5f),
                Source.Width,
                Source.Height);
        }

        internal void MarkForDestroy() 
        {
            MarkedForDestroy = true;
        }
    }
}
