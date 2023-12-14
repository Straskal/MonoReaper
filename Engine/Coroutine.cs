using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public sealed class Coroutine
    {
        private readonly Stack<IEnumerator> _routines = new();

        public Coroutine(IEnumerator routine) 
        {
            _routines.Push(routine);
        }

        /// <summary>
        /// Returns true if the coroutine has finished
        /// </summary>
        public bool IsFinished 
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true when the coroutine is finished.
        /// </summary>
        /// <returns></returns>
        internal bool Update() 
        {
            if (!_routines.Peek().MoveNext())
            {
                _routines.Pop();

                if (_routines.Count == 0)
                {
                    IsFinished = true;
                    return true;
                }
            }
            else if (_routines.Peek().Current is IEnumerator enumerator) 
            {
                _routines.Push(enumerator);
            }

            return false;
        }
    }
}
