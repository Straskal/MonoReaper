using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    public static class Singletons
    {
        public static Dictionary<Type, Singleton> _singletons = new Dictionary<Type, Singleton>();

        public static void Register(Singleton singleton) 
        {
            _singletons.Add(singleton.GetType(), singleton);
        }

        public static T Get<T>() where T : Singleton
        {
            return _singletons[typeof(T)] as T;
        }

        internal static void Tick(GameTime gameTime)
        {
            foreach (var singleton in _singletons.Values) 
            {
                singleton.Tick(gameTime);
            }
        }
    }
}
