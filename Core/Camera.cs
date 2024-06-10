using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class Camera
    {
        private Vector3 _translation;
        private Vector3 _scale;
        private Vector3 _center;

        private Matrix _translationMatrix;
        private Matrix _rotationMatrix;
        private Matrix _scaleMatrix;
        private Matrix _centerTranslationMatrix;

        private bool _isDirty;

        public Camera(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                _isDirty = true;
            }
        }

        private float zoom = 1f;
        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                _isDirty = true;
            }
        }

        private float rotation;
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                _isDirty = true;
            }
        }

        private Matrix transformationMatrix;
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

                    _center.X = Width * 0.5f;
                    _center.Y = Height * 0.5f;
                    _center.Z = 0f;

                    Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                    Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                    Matrix.CreateScale(ref _scale, out _scaleMatrix);
                    Matrix.CreateTranslation(ref _center, out _centerTranslationMatrix);

                    transformationMatrix = _translationMatrix * _rotationMatrix * _scaleMatrix * _centerTranslationMatrix;
                    InverseTransformationMatrix = Matrix.Invert(transformationMatrix);
                    _isDirty = false;
                }

                return transformationMatrix;
            }
        }

        public Matrix InverseTransformationMatrix { get; private set; }

        public Vector2 ToScreen(Vector2 position)
        {
            return Vector2.Transform(position, TransformationMatrix);
        }

        public Vector2 ToWorld(Vector2 position)
        {
            return Vector2.Transform(position, InverseTransformationMatrix);
        }
    }
}
