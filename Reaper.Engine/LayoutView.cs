using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine
{
    /// <summary>
    /// The layout view is essentially the camera.
    /// </summary>
    public class LayoutView
    {
        private readonly GameWindow _window;
        private readonly Layout _layout;
        private readonly GraphicsDevice _gpu;
        private readonly SpriteBatch _batch;

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
        private Effect _currentEffect;

        public LayoutView(MainGame game, Layout layout)
        {
            _window = game.Window;
            _layout = layout;
            _gpu = game.GraphicsDevice;
            _batch = new SpriteBatch(_gpu);

            Width = game.ViewportWidth;
            Height = game.ViewportHeight;

            Zoom = 1f;
            Rotation = 0.0f;
            Position = new Vector2(Width * 0.5f, Height * 0.5f);
        }

        public SpriteBatch SpriteBatch => _batch;
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        public Vector2 Position 
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 OffsetPosition => ClampViewToLayout(_position + new Vector2(OffsetX, OffsetY));

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Width { get; }
        public int Height { get; }

        public Matrix TransformationMatrix
        {
            get
            {
                // Offset the view by it's position.
                _translation.X = -OffsetPosition.X;
                _translation.Y = -OffsetPosition.Y;

                // Scale the view by it's zoom factor.
                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1f;

                // Center the view's position relative to it's resolution.
                _resolution.X = Width * 0.5f;
                _resolution.Y = Height * 0.5f;
                _resolution.Z = 0;

                // Scale the view to our virtual resolution.
                _resolutionScale.X = (float)_window.ClientBounds.Width / Width;
                _resolutionScale.Y = (float)_window.ClientBounds.Width / Width;
                _resolutionScale.Z = 1f;

                Matrix.CreateTranslation(ref _translation, out _translationMatrix);
                Matrix.CreateRotationZ(Rotation, out _rotationMatrix);
                Matrix.CreateScale(ref _scale, out _scaleMatrix);
                Matrix.CreateTranslation(ref _resolution, out _resolutionTranslationMatrix);
                Matrix.CreateScale(ref _resolutionScale, out _resolutionScaleMatrix);

                return _translationMatrix
                    * _rotationMatrix
                    * _scaleMatrix
                    * _resolutionTranslationMatrix
                    * _resolutionScaleMatrix;
            }
        }

        internal void BeginDraw()
        {
            _currentEffect = null;

            CreatePillarBoxes();
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
            HandleEffectChange(effect);

            _batch.Draw(texture, destination, source, color, 0, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, bool flipped, Effect effect = null)
        {
            HandleEffectChange(effect);

            _batch.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        private void HandleEffectChange(Effect effect) 
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

        internal void Unload() 
        {
            _batch.Dispose();
        }

        private void CreatePillarBoxes()
        {
            _gpu.Viewport = GetFullViewport();
            _gpu.Clear(Color.Black);
            _gpu.Viewport = GetLargestVirtualViewport();
        }

        private Vector2 ClampViewToLayout(Vector2 position)
        {
            var min = new Vector2(Width * 0.5f, Height * 0.5f);
            var max = new Vector2(_layout.Width - Width * 0.5f, _layout.Height - Height * 0.5f);

            return Vector2.Clamp(position, min, max);
        }

        private Viewport GetFullViewport()
        {
            return new Viewport
            {
                X = 0,
                Y = 0,
                Width = _window.ClientBounds.Width,
                Height = _window.ClientBounds.Height
            };
        }

        private Viewport GetLargestVirtualViewport()
        {
            var targetAspectRatio = Width / (float)Height;
            var width = _window.ClientBounds.Width;
            var height = (int)(width / targetAspectRatio + 0.5f);

            if (height > _window.ClientBounds.Height)
            {
                height = _window.ClientBounds.Height;
                width = (int)(height * targetAspectRatio + .5f);
            }

            return new Viewport
            {
                X = (_window.ClientBounds.Width / 2) - (width / 2),
                Y = (_window.ClientBounds.Height / 2) - (height / 2),
                Width = width,
                Height = height
            };
        }
    }
}
