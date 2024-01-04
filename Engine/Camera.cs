using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class Camera
    {
        private readonly BackBuffer _backBuffer;

        private Vector3 _translation;
        private Vector3 _scale;
        private Vector3 _center;
        private Matrix _translationMatrix;
        private Matrix _rotationMatrix;
        private Matrix _scaleMatrix;
        private Matrix _centerTranslationMatrix;
        private bool _isDirty;

        public Camera(BackBuffer backBuffer)
        {
            _backBuffer = backBuffer;
        }

        private Vector2 _position;

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

                    _center.X = _backBuffer.Width * 0.5f;
                    _center.Y = _backBuffer.Height * 0.5f;
                    _center.Z = 0f;

                    Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                    Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                    Matrix.CreateScale(ref _scale, out _scaleMatrix);
                    Matrix.CreateTranslation(ref _center, out _centerTranslationMatrix);

                    _transformationMatrix = _translationMatrix * _rotationMatrix * _scaleMatrix * _centerTranslationMatrix * _backBuffer.RendererScaleMatrix;
                    _isDirty = false;
                }

                return _transformationMatrix;
            }
        }

        public Vector2 ToScreen(Vector2 position)
        {
            position.X += _backBuffer.LetterboxViewport.X;
            position.Y += _backBuffer.LetterboxViewport.Y;

            return Vector2.Transform(position, TransformationMatrix * _backBuffer.VirtualBackBufferScaleMatrix);
        }

        public Vector2 ToWorld(Vector2 position)
        {
            position.X -= _backBuffer.LetterboxViewport.X;
            position.Y -= _backBuffer.LetterboxViewport.Y;

            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix * _backBuffer.VirtualBackBufferScaleMatrix));
        }
    }
}
