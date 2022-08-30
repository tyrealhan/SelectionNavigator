using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCan.SelectionNavigator
{
    [Serializable]
    public class HistoryQueue<T>
    {
        public static HistoryQueue<T> FromArray(T[] array, int current, int capacity)
        {
            var hb = new HistoryQueue<T>(capacity);
            if (array == null) return hb;
            foreach (T item in array) hb.Push(item);
            hb.current = current;
            return hb;
        }

        private Queue<T> queue;
        private int capacity;
        private int current = -1;

        public int Size { get { return queue.Count; } }

        public HistoryQueue(int capacity)
        {
            queue = new Queue<T>(capacity);
            this.capacity = capacity;
        }

        public void Push(T item)
        {
            queue.Enqueue(item);
            if (queue.Count > capacity) queue.Dequeue();
            current = queue.Count - 1;
        }

        public T Current()
        {
            if (current < 0) return default;
            return queue.ToArray()[current];
        }

        public T Previous()
        {
            if (current < 0) return default;
            current = Math.Max(0, current - 1);
            return queue.ToArray()[current];
        }

        public T Next()
        {
            if (current < 0) return default;
            current = Math.Min(current + 1, queue.Count - 1);
            return queue.ToArray()[current];
        }

        public T Last()
        {
            if (current < 0) return default;
            return queue.ToArray()[queue.Count - 1];
        }

        public void SetCurrent(int index)
        {
            current = index;
        }

        public int ResetCurrent()
        {
            current = queue.Count - 1;
            if (current < 0) return 0;
            return current;
        }

        public void Clear()
        {
            this.current = -1;
            queue.Clear();
        }

        public T[] ToArray()
        {
            return queue.ToArray();
        }

        public int GetCurrentIndex()
        {
            return current < 0 ? -1 : current;
        }
    }

}