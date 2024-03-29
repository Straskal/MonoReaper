﻿using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// Singletons exist once per game and receive callbacks similiar to behaviors.
    /// 
    /// They're good for services that the game may require. i.e. inventory system, layout management, etc...
    /// </summary>
    public abstract class Singleton
    {
        public Singleton(MainGame game) 
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public MainGame Game { get; }
        public Layout CurrentLayout => Game.CurrentLayout;

        public virtual void Load() { }
        public virtual void OnLayoutStarted() { }
        public virtual void HandleInput(GameTime gameTime) { }
        public virtual void Tick(GameTime gameTime) { }
        public virtual void Draw(Renderer renderer) { }
        public virtual void DebugDraw(Renderer renderer) { }
        public virtual void DrawGUI(Renderer renderer) { }
        public virtual void OnLayoutEnded() { }
    }
}
