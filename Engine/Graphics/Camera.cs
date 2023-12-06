using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public sealed class Camera
    {
        private Vector3 _translation = Vector3.Zero;
        private Vector3 _scale = Vector3.Zero;
        private Vector3 _resolutionTranslation = Vector3.Zero;
        private Matrix _translationMatrix = Matrix.Identity;
        private Matrix _rotationMatrix = Matrix.Identity;
        private Matrix _scaleMatrix = Matrix.Identity;
        private Matrix _resolutionTranslationMatrix = Matrix.Identity;

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

        public Matrix TransformationMatrix
        {
            get
            {
                // Move
                _translation.X = -Position.X;
                _translation.Y = -Position.Y;
                _translation.Z = 0f;

                // Zoom
                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1f;

                // Center based on resolution
                _resolutionTranslation.X = Resolution.Width * 0.5f;
                _resolutionTranslation.Y = Resolution.Height * 0.5f;
                _resolutionTranslation.Z = 0f;

                Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                Matrix.CreateScale(ref _scale, out _scaleMatrix);
                Matrix.CreateTranslation(ref _resolutionTranslation, out _resolutionTranslationMatrix);

                return _translationMatrix
                    * _rotationMatrix
                    * _scaleMatrix
                    * _resolutionTranslationMatrix
                    * Resolution.TransformationMatrix;
            }
        }

        public Vector2 ToScreen(Vector2 position)
        {
            // Need to account for viewport width and height. Haven't figured it out yet.

            return Vector2.Transform(position, TransformationMatrix);
        }

        public Vector2 ToWorld(Vector2 position)
        {
            // Need to account for viewport width and height. Don't really like that it's right here.
            position.X -= App.Graphics.Viewport.X;
            position.Y -= App.Graphics.Viewport.Y;

            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix));
        }
    }
}
