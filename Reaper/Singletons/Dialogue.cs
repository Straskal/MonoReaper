using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using Reaper.Engine;
using System;

namespace Reaper.Singletons
{
    public class LetterboxPart
    {
        public Vector2 Position { get; set; }
    }

    public class Dialogue : Singleton
    {
        private readonly IGame _game;
        private readonly Tweener _tweener;
        private readonly LetterboxPart _topLetterbox = new LetterboxPart();
        private readonly LetterboxPart _bottomLetterbox = new LetterboxPart();

        private Action<GameTime> _currentAction;

        public Dialogue(IGame game) 
        {
            _game = game;
            _tweener = new Tweener();

            _topLetterbox.Position = new Vector2(0, -32);
            _bottomLetterbox.Position = new Vector2(0, 180 + 16);
        }

        public void StartDialogue() 
        {
            if (IsComplete())
                DoLetterbox();
        }

        public void EndDialogue()
        {
            if (IsComplete())
                UndoLetterbox();
        }

        public override void Tick(GameTime gameTime)
        {
            _currentAction?.Invoke(gameTime);
        }

        public override void Draw(LayoutView view)
        {
            var winWidth = _game.Window.ClientBounds.Width;
            var winHeight = _game.Window.ClientBounds.Height;

            view.DrawRectangle(new Rectangle(view.Left, (int)_topLetterbox.Position.Y, view.Width, 32), Color.Black);
            view.DrawRectangle(new Rectangle(view.Left, (int)_bottomLetterbox.Position.Y, view.Width, 32), Color.Black);
        }

        private Tween _tween1;
        private Tween _tween2;

        private void DoLetterbox()
        {
            _tween1 = _tweener.TweenTo(_topLetterbox, lb => lb.Position, new Vector2(0, 0), 1).Easing(EasingFunctions.ExponentialOut);
            _tween2 = _tweener.TweenTo(_bottomLetterbox, lb => lb.Position, new Vector2(0, 148 + 16), 1).Easing(EasingFunctions.ExponentialOut); 
            _currentAction = DisplayLetterbox;
        }

        private void DisplayLetterbox(GameTime gameTime) 
        {
            _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void UndoLetterbox()
        {
            _tween1 = _tweener.TweenTo(_topLetterbox, lb => lb.Position, new Vector2(0, -32), 1).Easing(EasingFunctions.ExponentialOut);
            _tween2 = _tweener.TweenTo(_bottomLetterbox, lb => lb.Position, new Vector2(0, 180 + 16), 1).Easing(EasingFunctions.ExponentialOut);
            _currentAction = DisplayLetterbox;
        }

        private bool IsComplete()
        {
            return _tween1 != null ? _tween1.IsComplete : true;
        }
    }
}
