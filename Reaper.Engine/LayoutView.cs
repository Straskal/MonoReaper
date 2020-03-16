using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine
{
    public class LayoutView
    {
        private readonly GameWindow _window;
        private readonly GraphicsDevice _gpu;
        private readonly SpriteBatch _batch;

        private Matrix _translationMat3 = Matrix.Identity;
        private Matrix _rotationMat3 = Matrix.Identity;
        private Matrix _scaleMat3 = Matrix.Identity;
        private Matrix _resolutionTranslationMat4 = Matrix.Identity;
        private Matrix _resolutionScaleMat4 = Matrix.Identity;

        private Vector3 _translation = Vector3.Zero;
        private Vector3 _scale = Vector3.Zero;
        private Vector3 _resolution = Vector3.Zero;

        private Effect _currentEffect;

        public LayoutView(MainGame game)
        {
            _window = game.Window;
            _gpu = game.GraphicsDevice;
            _batch = new SpriteBatch(_gpu);

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
                _translation.X = -Position.X;
                _translation.Y = -Position.Y;

                Matrix.CreateTranslation(ref _translation, out _translationMat3);
                Matrix.CreateRotationZ(Rotation, out _rotationMat3);

                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1;

                Matrix.CreateScale(ref _scale, out _scaleMat3);

                _resolution.X = VirtualWidth * 0.5f;
                _resolution.Y = VirtualHeight * 0.5f;
                _resolution.Z = 0;

                Matrix.CreateTranslation(ref _resolution, out _resolutionTranslationMat4);

                Vector3 resolutionScaleVector = new Vector3((float)ScreenWidth / VirtualWidth, (float)ScreenWidth / VirtualWidth, 1f);
                Matrix.CreateScale(ref resolutionScaleVector, out _resolutionScaleMat4);

                return _translationMat3
                    * _rotationMat3
                    * _scaleMat3
                    * _resolutionTranslationMat4
                    * _resolutionScaleMat4;
            }
        }

        internal void BeginDraw()
        {
            _currentEffect = null;

            SyncViewport();
            PrepareBatch();
        }

        private void PrepareBatch() 
        {
            _batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                _currentEffect,
                TransformationMatrix
            );
        }

        public void Draw(Texture2D texture, Rectangle source, Rectangle destination, Color color, bool flipped, Effect effect = null) 
        {
            ChangeEffect(effect);

            _batch.Draw(texture, destination, source, color, 0, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, bool flipped, Effect effect = null)
        {
            ChangeEffect(effect);

            _batch.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        private void ChangeEffect(Effect effect) 
        {
            if (_currentEffect != effect)
            {
                _currentEffect = effect;

                EndDraw();
                PrepareBatch();
            }
        }

        internal void EndDraw() 
        {
            _batch.End();
        }

        private void SyncViewport()
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
