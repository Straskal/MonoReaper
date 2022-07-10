using Microsoft.Xna.Framework;
using System;

namespace Core.Graphics
{
    public sealed class Camera
    {
        private readonly Level _level;

        private Matrix _translationMatrix = Matrix.Identity;
        private Matrix _rotationMatrix = Matrix.Identity;
        private Matrix _scaleMatrix = Matrix.Identity;
        private Matrix _resolutionTranslationMatrix = Matrix.Identity;
        private Matrix _resolutionScaleMatrix = Matrix.Identity;
        private Vector3 _translation = Vector3.Zero;
        private Vector3 _scale = Vector3.Zero;
        private Vector3 _resolution = Vector3.Zero;
        private Vector3 _resolutionScale = Vector3.Zero;
        private Vector2 _position;
        private Vector2 _offsetPosition;

        public Camera(Level level)
        {
            _level = level;

            Width = App.ViewportWidth;
            Height = App.ViewportHeight;
            Zoom = 1f;
            Rotation = 0.0f;
            Position = new Vector2(Width * 0.5f, Height * 0.5f);
        }

        public float Zoom { get; set; }
        public float Rotation { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                //_position = ClampViewToLayout(value);
                _offsetPosition = _position + new Vector2(OffsetX, OffsetY);
            }
        }

        public int Left => (int)(Position.X - Width * 0.5f);
        public int Right => (int)(Position.X + Width * 0.5f);

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Width { get; }
        public int Height { get; }

        public Matrix TransformationMatrix
        {
            get
            {
                // Offset the view by it's position.
                _translation.X = (int)Math.Floor(-_offsetPosition.X);
                _translation.Y = (int)Math.Floor(-_offsetPosition.Y);

                // Scale the view by it's zoom factor.
                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1f;

                // Center the view's position relative to it's resolution.
                _resolution.X = Width * 0.5f;
                _resolution.Y = Height * 0.5f;
                _resolution.Z = 0;

                // Scale the view to our virtual resolution.
                _resolutionScale.X = (float)App.Graphics.PresentationParameters.BackBufferWidth / Width;
                _resolutionScale.Y = (float)App.Graphics.PresentationParameters.BackBufferWidth / Width;
                _resolutionScale.Z = 1f;

                Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                Matrix.CreateScale(ref _scale, out _scaleMatrix);
                Matrix.CreateTranslation(ref _resolution, out _resolutionTranslationMatrix);
                //Matrix.CreateScale(ref _resolutionScale, out _resolutionScaleMatrix);

                return _translationMatrix
                    * _rotationMatrix
                    * _scaleMatrix
                    * _resolutionTranslationMatrix;
                    //* _resolutionScaleMatrix;
            }
        }

        /// <summary>
        /// Converts coordinates in screen space to world space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToWorld(Vector2 position)
        {
            return Vector2.Transform(position, TransformationMatrix);
        }

        /// <summary>
        /// Converts coordinates in world space to screen space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToScreen(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix));
        }

        /// <summary>
        /// Clamps the view to the inside of the layout. If the layout is smaller than the view, then the view is centered.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2 ClampViewToLayout(Vector2 position)
        {
            float xMin, xMax, yMin, yMax;

            if (Width >= _level.Width)
            {
                xMin = _level.Width * 0.5f;
                xMax = _level.Width * 0.5f;
            }
            else
            {
                xMin = Width * 0.5f;
                xMax = _level.Width - Width * 0.5f;
            }

            if (Height >= _level.Height)
            {
                yMin = _level.Height * 0.5f;
                yMax = _level.Height * 0.5f;
            }
            else
            {
                yMin = Width * 0.5f;
                yMax = _level.Height - Height * 0.5f;
            }

            var min = new Vector2(xMin, yMin);
            var max = new Vector2(xMax, yMax);

            return Vector2.Clamp(position, min, max);
        }
    }
}
