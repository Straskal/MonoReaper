﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Engine
{
    public class LayoutView
    {
        private readonly GameWindow _window;
        private readonly Layout _layout;
        private readonly GraphicsDevice _gpu;
        private readonly SpriteBatch _batch;

        private Matrix _translationMat3 = Matrix.Identity;
        private Matrix _rotationMat3 = Matrix.Identity;
        private Matrix _scaleMat3 = Matrix.Identity;
        private Matrix _resolutionTranslationMat3 = Matrix.Identity;
        private Matrix _resolutionScaleMat3 = Matrix.Identity;

        private Vector3 _translation = Vector3.Zero;
        private Vector3 _scale = Vector3.Zero;
        private Vector3 _resolution = Vector3.Zero;

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
            set
            {
                var min = new Vector2(0 + Width * 0.5f, 0 + Height * 0.5f);
                var max = new Vector2(_layout.Width - Width * 0.5f, _layout.Height - Height * 0.5f);

                _position = Vector2.Clamp(value, min, max);
            }
        }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }
        public int Width { get; }
        public int Height { get; }

        public Matrix TransformationMatrix
        {
            get
            {
                _translation.X = -Position.X - OffsetX;
                _translation.Y = -Position.Y - OffsetY;

                Matrix.CreateTranslation(ref _translation, out _translationMat3);
                Matrix.CreateRotationZ(Rotation, out _rotationMat3);

                _scale.X = Zoom;
                _scale.Y = Zoom;
                _scale.Z = 1;

                Matrix.CreateScale(ref _scale, out _scaleMat3);

                _resolution.X = Width * 0.5f;
                _resolution.Y = Height * 0.5f;
                _resolution.Z = 0;

                Matrix.CreateTranslation(ref _resolution, out _resolutionTranslationMat3);

                Vector3 resolutionScaleVector = new Vector3((float)WindowWidth / Width, (float)WindowWidth / Width, 1f);
                Matrix.CreateScale(ref resolutionScaleVector, out _resolutionScaleMat3);

                return _translationMat3
                    * _rotationMat3
                    * _scaleMat3
                    * _resolutionTranslationMat3
                    * _resolutionScaleMat3;
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

        internal void Unload() 
        {
            _batch.Dispose();
        }

        private void CreatePillarBoxes()
        {
            WindowWidth = _window.ClientBounds.Width;
            WindowHeight = _window.ClientBounds.Height;

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
                Width = WindowWidth,
                Height = WindowHeight
            };
        }

        private Viewport GetLargestVirtualViewport()
        {
            var targetAspectRatio = Width / (float)Height;
            var width = WindowWidth;
            var height = (int)(width / targetAspectRatio + 0.5f);

            if (height > WindowHeight)
            {
                height = WindowHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            return new Viewport
            {
                X = (WindowWidth / 2) - (width / 2),
                Y = (WindowHeight / 2) - (height / 2),
                Width = width,
                Height = height
            };
        }
    }
}
