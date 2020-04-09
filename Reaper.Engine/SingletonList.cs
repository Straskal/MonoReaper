﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
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

        internal void Tick(GameTime gameTime)
        {
            foreach (var singleton in _singletons.Values) 
                singleton.Tick(gameTime);
        }

        internal void Draw(Renderer renderer, bool isDebugging)
        {
            foreach (var singleton in _singletons.Values)
                singleton.Draw(renderer, isDebugging);
        }
    }
}
