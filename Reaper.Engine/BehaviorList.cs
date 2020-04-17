using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    public sealed class BehaviorList
    {
        private readonly WorldObject _owner;
        private readonly List<Behavior> _behaviors;

        public BehaviorList(WorldObject owner) 
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _behaviors = new List<Behavior>();
        }

        public T Get<T>() where T : class
        {
            return _behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        public bool TryGet<T>(out T behavior) where T : class
        {
            behavior = _behaviors.FirstOrDefault(b => b is T) as T;
            return behavior != null;
        }

        internal void Add(Func<WorldObject, Behavior> createFunc)
        {
            _behaviors.Add(createFunc?.Invoke(_owner));
        }

        internal void Load(ContentManager contentManager)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Load(contentManager);
            }
        }

        internal void OnCreated()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerCreated();
            }
        }

        internal void OnLayoutStarted()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnLayoutStarted();
            }
        }

        internal void Tick(GameTime gameTime)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Tick(gameTime);
            }
        }

        internal void PostTick(GameTime gameTime)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.PostTick(gameTime);
            }
        }

        internal void Draw(Renderer renderer)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Draw(renderer);
            }
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.DebugDraw(renderer);
            }
        }

        internal void OnDestroyed()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnOwnerDestroyed();
            }
        }
    }
}
