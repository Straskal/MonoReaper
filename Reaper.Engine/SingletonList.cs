using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    /// <summary>
    /// The list of all of the game's singletons.
    /// </summary>
    public class SingletonList
    {
        public Dictionary<Type, Singleton> _singletons = new Dictionary<Type, Singleton>();

        public void Register(Singleton singleton) 
        {
            var type = singleton.GetType();

            if (_singletons.ContainsKey(type))
                throw new ArgumentException($"{type} is already a registered singleton.");

            _singletons.Add(singleton.GetType(), singleton);
        }

        public T Get<T>() where T : Singleton
        {
            if (_singletons.TryGetValue(typeof(T), out var singleton))
                return singleton as T;

            throw new ArgumentException($"{typeof(T)} is not a registered singleton.");
        }

        internal void Load()
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.Load();
            }
        }

        internal void OnLayoutStarted()
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.OnLayoutStarted();
            }
        }

        internal void HandleInput(GameTime gameTime) 
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.HandleInput(gameTime);
            }
        }

        internal void Tick(GameTime gameTime)
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.Tick(gameTime);
            }
        }

        internal void Draw(Renderer renderer, bool isDebugging)
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.Draw(renderer);
            }

            if (isDebugging)
            {
                foreach (var singleton in _singletons.Values)
                {
                    singleton.DebugDraw(renderer);
                }
            }
        }

        internal void DrawGUI(Renderer renderer)
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.DrawGUI(renderer);
            }
        }

        internal void OnLayoutEnded()
        {
            foreach (var singleton in _singletons.Values)
            {
                singleton.OnLayoutEnded();
            }
        }
    }
}
