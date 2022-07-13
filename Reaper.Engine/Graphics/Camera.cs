using System;
using Microsoft.Xna.Framework;

namespace Core.Graphics
{
    public sealed class Camera
    {
        private Vector3 _translation = Vector3.Zero;
        private Vector3 _scale = Vector3.Zero;

        private Matrix _translationMatrix = Matrix.Identity;
        private Matrix _rotationMatrix = Matrix.Identity;
        private Matrix _scaleMatrix = Matrix.Identity;

        public Camera(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1f;
        public float Rotation { get; set; } = 0f;
        public int Width { get; }
        public int Height { get; }

        public int Left => (int)(Position.X - Width * 0.5f);
        public int Right => (int)(Position.X + Width * 0.5f);

        public Matrix TransformationMatrix
        {
            get
            {
                // Move
                _translation.X = -Position.X;
                _translation.Y = -Position.Y;
                _translation.Z = 0f;

                // Center
                _translation.X += Width * 0.5f;
                _translation.Y += Height * 0.5f;

                // Zoom
                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1f;

                Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                Matrix.CreateScale(ref _scale, out _scaleMatrix);

                return _translationMatrix
                    * _rotationMatrix
                    * _scaleMatrix;
            }
        }

        public Vector2 ToWorld(Vector2 position)
        {
            return Vector2.Transform(position, TransformationMatrix);
        }

        public Vector2 ToScreen(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix));
        }

        /// <summary>
        /// Clamps the view to the inside of the layout. If the layout is smaller than the view, then the view is centered.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //private Vector2 ClampToBounds(Vector2 position)
        //{
        //    float xMin, xMax, yMin, yMax;

        //    if (Width >= _level.Width)
        //    {
        //        xMin = _level.Width * 0.5f;
        //        xMax = _level.Width * 0.5f;
        //    }
        //    else
        //    {
        //        xMin = Width * 0.5f;
        //        xMax = _level.Width - Width * 0.5f;
        //    }

        //    if (Height >= _level.Height)
        //    {
        //        yMin = _level.Height * 0.5f;
        //        yMax = _level.Height * 0.5f;
        //    }
        //    else
        //    {
        //        yMin = Width * 0.5f;
        //        yMax = _level.Height - Height * 0.5f;
        //    }

        //    var min = new Vector2(xMin, yMin);
        //    var max = new Vector2(xMax, yMax);

        //    return Vector2.Clamp(position, min, max);
        //}
    }
}
