using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    /// <summary>
    /// This class can manipulate the view.
    /// </summary>
    public sealed class Camera
    {
        private readonly VirtualScreen _virtualScreen;

        private Vector3 _translation;
        private Vector3 _scale;
        private Vector3 _center;
        private Matrix _translationMatrix;
        private Matrix _rotationMatrix;
        private Matrix _scaleMatrix;
        private Matrix _centerTranslationMatrix;
        private bool _isDirty;

        public Camera(VirtualScreen virtualScreen)
        {
            _virtualScreen = virtualScreen;
        }

        private Vector2 _position;

        /// <summary>
        /// Gets or sets the camera's position
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _isDirty = true;
            }
        }

        private float _zoom = 1f;

        /// <summary>
        /// Gets or sets the camera's zoom
        /// </summary>
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                _isDirty = true;
            }
        }

        private float _rotation;

        /// <summary>
        /// Gets or sets the camera's rotation
        /// </summary>
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _isDirty = true;
            }
        }

        private Matrix _transformationMatrix;

        /// <summary>
        /// Gets the camera's transformation matrix
        /// </summary>
        public Matrix TransformationMatrix
        {
            get
            {
                if (_isDirty)
                {
                    _translation.X = -Position.X;
                    _translation.Y = -Position.Y;
                    _translation.Z = 0f;

                    _scale.X = Zoom;
                    _scale.Y = Zoom;
                    _scale.Z = 1f;

                    _center.X = _virtualScreen.Width * 0.5f;
                    _center.Y = _virtualScreen.Height * 0.5f;
                    _center.Z = 0f;

                    Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                    Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                    Matrix.CreateScale(ref _scale, out _scaleMatrix);
                    Matrix.CreateTranslation(ref _center, out _centerTranslationMatrix);

                    _transformationMatrix = _translationMatrix * _rotationMatrix * _scaleMatrix * _centerTranslationMatrix * _virtualScreen.RendererScaleMatrix;
                    _isDirty = false;
                }

                return _transformationMatrix;
            }
        }

        /// <summary>
        /// Transforms a world position to screen position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToScreen(Vector2 position)
        {
            position.X += _virtualScreen.LetterboxViewport.X;
            position.Y += _virtualScreen.LetterboxViewport.Y;

            return Vector2.Transform(position, TransformationMatrix * _virtualScreen.VirtualBackBufferScaleMatrix);
        }

        /// <summary>
        /// Transforms a screen position to world position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToWorld(Vector2 position)
        {
            position.X -= _virtualScreen.LetterboxViewport.X;
            position.Y -= _virtualScreen.LetterboxViewport.Y;

            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix * _virtualScreen.VirtualBackBufferScaleMatrix));
        }
    }
}
