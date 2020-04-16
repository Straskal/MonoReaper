using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// The game's renderer. Should only be used when it is passed through Draw(...) methods.
    /// </summary>
    public class Renderer
    {
        private readonly MainGame _game;
        private readonly SpriteBatch _batch;
        private readonly Texture2D _texture;

        private Effect _currentEffect;

        internal Renderer(MainGame game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _batch = new SpriteBatch(game.GraphicsDevice);
            _texture = new Texture2D(game.GraphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        internal void BeginDraw()
        {
            _currentEffect = null;

            CreateBlackBars();
            PrepareBatch();
        }

        private void PrepareBatch()
        {
            _batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                _currentEffect,
                _game.CurrentLayout.View.TransformationMatrix
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

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            _batch.Draw(_texture, rectangle, null, color);
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

        private void CreateBlackBars()
        {
            _game.GraphicsDevice.Viewport = GetFullViewport();
            _game.GraphicsDevice.Clear(Color.Black);
            _game.GraphicsDevice.Viewport = GetLargestVirtualViewport();
        }

        private Viewport GetFullViewport()
        {
            return new Viewport
            {
                X = 0,
                Y = 0,
                Width = _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = _game.GraphicsDevice.PresentationParameters.BackBufferHeight
            };
        }

        private Viewport GetLargestVirtualViewport()
        {
            // Start off assuming letterbox.
            float targetAspectRatio = _game.CurrentLayout.View.Width / (float)_game.CurrentLayout.View.Height;
            int screenWidth = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int width = screenWidth;
            int height = (int)(screenWidth / targetAspectRatio + 0.5f);

            // Check if we need pillarbox instead.
            if (height > screenHeight)
            {
                height = screenHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            return new Viewport
            {
                X = screenWidth / 2 - width / 2,
                Y = screenHeight / 2 - height / 2,
                Width = width,
                Height = height
            };
        }
    }
}
