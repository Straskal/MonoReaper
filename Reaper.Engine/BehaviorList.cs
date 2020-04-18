using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    public sealed class BehaviorList : IEnumerable<Behavior>
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
            return _behaviors.FirstOrDefault(b => b is T) as T;
        }

        public bool TryGet<T>(out T behavior) where T : class
        {
            behavior = _behaviors.FirstOrDefault(b => b is T) as T;
            return behavior != null;
        }

        public T Require<T>() where T : class
        {
            return _behaviors.FirstOrDefault(b => b is T) as T ?? throw new Exception($"Required behavior of type {typeof(T).Name} is missing.");
        }

        internal void Add(Func<WorldObject, Behavior> createFunc)
        {
            _behaviors.Add(createFunc?.Invoke(_owner));
        }

        public IEnumerator<Behavior> GetEnumerator()
        {
            foreach (var behavior in _behaviors) 
            {
                yield return behavior;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
