using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItsGood
{
    public class LayoutView
    {
        private readonly GameWindow _window;
        private readonly GraphicsDevice _gpu;

        private Matrix _camTranslationMatrix = Matrix.Identity;
        private Matrix _camRotationMatrix = Matrix.Identity;
        private Matrix _camScaleMatrix = Matrix.Identity;
        private Matrix _resTranslationMatrix = Matrix.Identity;

        private Vector3 _camTranslationVector = Vector3.Zero;
        private Vector3 _camScaleVector = Vector3.Zero;
        private Vector3 _resTranslationVector = Vector3.Zero;

        public LayoutView(MainGame game)
        {
            _window = game.Window;
            _gpu = game.GraphicsDevice;

            VirtualWidth = game.ViewportWidth;
            VirtualHeight = game.ViewportHeight;

            Zoom = 1f;
            Rotation = 0.0f;
            Position = new Vector2(VirtualWidth * 0.5f, VirtualHeight * 0.5f);
        }

        public float Zoom { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int VirtualWidth { get; }
        public int VirtualHeight { get; }

        public Matrix TransformationMatrix
        {
            get
            {
                _camTranslationVector.X = -Position.X;
                _camTranslationVector.Y = -Position.Y;

                Matrix.CreateTranslation(ref _camTranslationVector, out _camTranslationMatrix);
                Matrix.CreateRotationZ(Rotation, out _camRotationMatrix);

                _camScaleVector.X = Zoom;
                _camScaleVector.Y = Zoom;
                _camScaleVector.Z = 1;

                Matrix.CreateScale(ref _camScaleVector, out _camScaleMatrix);

                _resTranslationVector.X = VirtualWidth * 0.5f;
                _resTranslationVector.Y = VirtualHeight * 0.5f;
                _resTranslationVector.Z = 0;

                Matrix.CreateTranslation(ref _resTranslationVector, out _resTranslationMatrix);

                return _camTranslationMatrix
                    * _camRotationMatrix
                    * _camScaleMatrix
                    * _resTranslationMatrix
                    * ResolutionScalematrix;
            }
        }

        public Matrix ResolutionScalematrix
        {
            get
            {
                var scaleX = (float)ScreenWidth / VirtualWidth;
                var scaleY = (float)ScreenWidth / VirtualWidth;

                return Matrix.CreateScale(scaleX, scaleY, 1.0f);
            }
        }

        public void Sync()
        {
            ScreenWidth = _window.ClientBounds.Width;
            ScreenHeight = _window.ClientBounds.Height;

            _gpu.Viewport = GetFullViewport();
            _gpu.Clear(Color.Black);
            _gpu.Viewport = GetLargestVirtualViewport();
        }

        private Viewport GetFullViewport()
        {
            return new Viewport
            {
                X = 0,
                Y = 0,
                Width = ScreenWidth,
                Height = ScreenHeight
            };
        }

        private Viewport GetLargestVirtualViewport()
        {
            var targetAspectRatio = VirtualWidth / (float)VirtualHeight;
            var width = ScreenWidth;
            var height = (int)(width / targetAspectRatio + 0.5f);

            if (height > ScreenHeight)
            {
                height = ScreenHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            return new Viewport
            {
                X = (ScreenWidth / 2) - (width / 2),
                Y = (ScreenHeight / 2) - (height / 2),
                Width = width,
                Height = height
            };

        }
    }
}
