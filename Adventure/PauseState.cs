﻿using Microsoft.Xna.Framework;
using Engine;
using Engine.Graphics;

namespace Adventure
{
    internal class PauseState : GameState
    {
        public PauseState(App application) : base(application)
        {
        }

        public override bool ShouldUpdateBelow
        {
            // Don't update the states below. This is what pauses the game.
            get => false;
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw();
            renderer.DrawRectangle(0, 0, Application.ResolutionWidth, Application.ResolutionHeight, new Color(Color.Black, 0.4f));
            renderer.DrawString(SharedContent.Font, "Paused", 100f, 100f, Color.White);
            GUI.Start();
            if (GUI.PrimaryButton(1, "Main menu", 100, 150))
            {
                Application.Stack.SetTop(new MainMenuState(Application));
            }
            renderer.EndDraw();
        }
    }
}
