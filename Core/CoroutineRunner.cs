using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public sealed class CoroutineRunner
    {
        private readonly List<Coroutine> coroutines = new();
        private readonly List<Coroutine> coroutinesToRemove = new();

        public Coroutine Start(IEnumerator enumerator) 
        {
            var coroutine = new Coroutine(enumerator);
            coroutines.Add(coroutine);
            return coroutine;
        }

        public void Stop(Coroutine coroutine) 
        {
            coroutinesToRemove.Add(coroutine);
        }

        public void Update() 
        {
            for (int i = 0; i < coroutines.Count; i++) 
            {
                if (coroutines[i].Update()) 
                {
                    coroutinesToRemove.Add(coroutines[i]);
                }
            }

            foreach (var coroutine in coroutinesToRemove) 
            {
                coroutines.Remove(coroutine);
            }

            coroutinesToRemove.Clear();
        }
    }
}
