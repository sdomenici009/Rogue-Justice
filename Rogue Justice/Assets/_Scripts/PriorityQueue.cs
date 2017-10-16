using System;
using System.Collections.Generic;

public class PriorityQueue<T> {
    private List<PriorityData> data;

    private struct PriorityData {
        public T data;
        public float priority;

        public PriorityData(T data, float priority) {
            this.data = data;
            this.priority = priority;
        }
    }

    public PriorityQueue() {
        this.data = new List<PriorityData>();
    }

    public void Enqueue(T item, float priority) {
        data.Add(new PriorityData(item, priority));
        int itemIndex = data.Count - 1;
        while (itemIndex > 0) {
            int parentIndex = (itemIndex - 1) / 2;
            if (data[itemIndex].priority <= data[parentIndex].priority) break;

            PriorityData tmp = data[itemIndex]; data[itemIndex] = data[parentIndex]; data[parentIndex] = tmp;
            itemIndex = parentIndex;
        }
    }

    public T Dequeue() {
        int lastIndex = data.Count - 1;
        PriorityData frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        --lastIndex;
        int parentIndex = 0;
        while (true) {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > lastIndex) break;

            int rightChildIndex = leftChildIndex + 1;
            if (rightChildIndex <= lastIndex && data[rightChildIndex].priority < data[leftChildIndex].priority) {
                leftChildIndex = rightChildIndex;
            }
            if (data[parentIndex].priority <= data[leftChildIndex].priority) break;

            PriorityData tmp = data[parentIndex]; data[parentIndex] = data[leftChildIndex]; data[leftChildIndex] = tmp;
            parentIndex = leftChildIndex;
        }
        return frontItem.data;
    }

    public T Peek() {
        T frontItem = data[0].data;
        return frontItem;
    }

    public int Count() {
        return data.Count;
    }
}

