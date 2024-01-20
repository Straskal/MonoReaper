using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class Camera
    {
        private Vector3 translation;
        private Vector3 scale;
        private Vector3 center;

        private Matrix translationMatrix;
        private Matrix rotationMatrix;
        private Matrix scaleMatrix;
        private Matrix centerTranslationMatrix;

        private bool isDirty;

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
                isDirty = true;
            }
        }

        private float zoom = 1f;
        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                isDirty = true;
            }
        }

        private float rotation;
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                isDirty = true;
            }
        }

        public RectangleF Bounds
        {
            get => new(Position.X - Width * 0.5f, Position.Y - Height * 0.5f, Width, Height);
        }

        private Matrix transformationMatrix;
        public Matrix TransformationMatrix
        {
            get
            {
                if (isDirty)
                {
                    translation.X = -Position.X;
                    translation.Y = -Position.Y;
                    translation.Z = 0f;

                    scale.X = Zoom;
                    scale.Y = Zoom;
                    scale.Z = 1f;

                    center.X = Width * 0.5f;
                    center.Y = Height * 0.5f;
                    center.Z = 0f;

                    Matrix.CreateTranslation(ref translation, out translationMatrix);
                    Matrix.CreateRotationZ(Rotation, out rotationMatrix);
                    Matrix.CreateScale(ref scale, out scaleMatrix);
                    Matrix.CreateTranslation(ref center, out centerTranslationMatrix);

                    transformationMatrix = translationMatrix * rotationMatrix * scaleMatrix * centerTranslationMatrix;
                    InverseTransformationMatrix = Matrix.Invert(transformationMatrix);
                    isDirty = false;
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
