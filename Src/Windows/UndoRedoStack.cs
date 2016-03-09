using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DevZest.Windows
{
    internal class UndoRedoStack<T>
    {
        private int _capacity;
        private List<T> _list = new List<T>();

        public UndoRedoStack(int capacity)
        {
            Capacity = capacity;
        }

        private int LastIndex
        {
            get { return _list.Count - 1; }
        }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                Debug.Assert(value >= 0);
                _capacity = value;
            }
        }

        private void EnforceCapacity()
        {
            if (Count > _capacity)
                _list.RemoveRange(0, Count - _capacity);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void Push(T value)
        {
            _list.Add(value);
            EnforceCapacity();
        }

        public T Peek()
        {
            Debug.Assert(Count > 0);
            return _list[LastIndex];
        }

        public T Pop()
        {
            Debug.Assert(Count > 0);
            T value = _list[LastIndex];
            _list.RemoveAt(LastIndex);
            return value;
        }
    }
}
