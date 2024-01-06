namespace DataStructures
{
    public sealed class CircularBuffer<T>
    {
        private T[] _array;
        private int _head;
        private int _tail;
        private int _peekTail;

        public int Tail => _tail;
        public int Head => _head;
        
        public CircularBuffer(int size, int initHeadOffset = 0)
        {
            _array = new T[size];
            _head = initHeadOffset;
            _tail = 0;
            _peekTail = _tail;
        }

        public void Push(T value)
        {
            int next = _head + 1;
            if (next >= _array.Length)
            {
                next = 0;
            }

            _array[_head] = value;
            _head = next;
        }

        public bool Peek(out T value)
        {
            if (_peekTail == _head)
            {
                value = default;
                return false;
            }

            int next = _peekTail + 1;
            if (next >= _array.Length)
            {
                next = 0;
            }
            
            value = _array[_peekTail];
            _peekTail = next;
            return true;
        }

        public void ResetPeekTail()
        {
            _peekTail = _tail;
        }

        public bool Pop(out T value)
        {
            if (_head == _tail)
            {
                value = default;
                return false;
            }
            
            int next = _tail + 1;
            if (next >= _array.Length)
            {
                next = 0;
            }

            value = _array[_tail];
            _tail = next;
            return true;
        }
    }
}