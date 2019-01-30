using System;

namespace EasySerialization.Json
{
    public class RingQueue<T>
    {
        private T[] _Items;
        private int _Head = 0;
        private int _Tail = 0;

        public RingQueue(int size)
        {
            _Items = new T[size];
        }

        public void Enqueue(T item)
        {
            _Items[_Head] = item;
            _Head = (_Head + 1) % _Items.Length;
            if (_Head == _Tail)
                _Tail = (_Tail + 1) % _Items.Length;
        }

        public T[] AllItems
        {
            get
            {
                if (_Tail <= _Head)
                {
                    int n = _Head - _Tail;
                    var result = new T[n];
                    Array.Copy(_Items, _Tail, result, 0, n);
                    return result;
                }
                else
                {
                    int n = _Head + _Items.Length - _Tail;
                    var result = new T[n];
                    Array.Copy(_Items, _Tail, result, 0, n - _Tail);
                    Array.Copy(_Items, 0, result, n - _Tail, _Head);
                    return result;
                }
            }
        }
    }
}
