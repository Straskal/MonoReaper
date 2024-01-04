using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public sealed class CoroutineRunner
    {
        private readonly List<Coroutine> _coroutines = new();
        private readonly List<Coroutine> _coroutinesToRemove = new();

        public Coroutine Start(IEnumerator enumerator) 
        {
            var coroutine = new Coroutine(enumerator);
            _coroutines.Add(coroutine);
            return coroutine;
        }

        public void Stop(Coroutine coroutine) 
        {
            _coroutinesToRemove.Add(coroutine);
        }

        public void Update() 
        {
            for (int i = 0; i < _coroutines.Count; i++) 
            {
                if (_coroutines[i].Update()) 
                {
                    _coroutinesToRemove.Add(_coroutines[i]);
                }
            }

            foreach (var coroutine in _coroutinesToRemove) 
            {
                _coroutines.Remove(coroutine);
            }

            _coroutinesToRemove.Clear();
        }
    }
}
