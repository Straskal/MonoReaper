using System;
using System.Collections;
using System.Collections.Generic;

namespace Adventure
{
    public class RingBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _items;

        private int _readPosition;
        private int _writePosition;
        private int _length;

        public RingBuffer(int capacity)
        {
            _items = new T[capacity];
        }

        public int Capacity { get { return _items.Length; } }
        public int Count { get { return _length; } }

        public void Clear() 
        {
            _readPosition = 0;
            _writePosition = 0;
            _length = 0;
        }

        public void Push(T item)
        {
            _items[_writePosition] = item;
            _writePosition = (_writePosition + 1) % Capacity;
            _length = (_length + 1) % Capacity;
        }

        public T Peek()
        {
            return _items[_readPosition];
        }

        public bool TryPeek(out T item) 
        {
            if (_length > 0) 
            {
                item = _items[_readPosition];
                return true;
            }
            item = default;
            return false;
        }

        public bool TryDequeue(out T item)
        {
            if (_length > 0)
            {
                item = Peek();
                _items[_readPosition] = default;
                _readPosition = (_readPosition + 1) % Capacity;
                _length--;
                return true;
            }
            item = default;
            return false;
        }

        public T Pop()
        {
            var item = Peek();
            _items[_readPosition] = default;
            _readPosition = (_readPosition + 1) % Capacity;
            _length--;
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly RingBuffer<T> _ringBuffer;
            private int _index;
            private T _current;

            internal Enumerator(RingBuffer<T> ringBuffer)
            {
                _ringBuffer = ringBuffer;
                _index = 0;
                _current = default;
            }

            public readonly T Current { get { return _current; } }

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                RingBuffer<T> ringBuffer = _ringBuffer;

                if ((uint)_index < (uint)ringBuffer._length)
                {
                    _current = ringBuffer._items[_index];
                    _index++;
                    return true;
                }

                return End();
            }

            private bool End()
            {
                _index = _ringBuffer._length + 1;
                _current = default;
                return false;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _ringBuffer._length + 1)
                    {
                        throw new InvalidOperationException();
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default;
            }
        }
    }
}
