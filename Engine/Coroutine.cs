using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public sealed class Coroutine
    {
        private readonly Stack<IEnumerator> routines = new();

        public Coroutine(IEnumerator routine)
        {
            routines.Push(routine);
        }

        public bool IsFinished { get; private set; }

        internal bool Update()
        {
            if (!routines.Peek().MoveNext())
            {
                routines.Pop();

                if (routines.Count == 0)
                {
                    IsFinished = true;
                }
            }
            else if (routines.Peek().Current is IEnumerator enumerator)
            {
                routines.Push(enumerator);
            }

            return IsFinished;
        }
    }
}
