using System;
using System.Collections.Generic;

namespace Stabiliteettilaskenta.Utility
{
    public class Heap<T> where T : IHeapItem<T>
    {
        //T[] items;
        List<T> items;
        int currentItemCount;

        //public MinHeap(int _maxHeapSize)
        //{
        //    items = new T[_maxHeapSize];
        //}
        public Heap()
        {
            items = new List<T>();
            currentItemCount = 0;
        }

        public void Clear()
        {
            items.Clear();
            currentItemCount = 0;
        }

        public void Add(T _item)
        {
            _item.HeapIndex = currentItemCount;
            items.Add(_item);       // List
            //items[currentItemCount] = _item;
            SortUp(_item);
            currentItemCount++;
        }
        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items.RemoveAt(currentItemCount);   // List
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }
        public void UpdateItem(T _item)
        {
            SortUp(_item);
            SortDown(_item);
        }
        public int Count { get { return currentItemCount; } }
        public bool Contains(T _item)
        {
            return Equals(items[_item.HeapIndex], _item);
        }
        void SortDown(T _item)
        {
            while (true)
            {
                int childIndexLeft = _item.HeapIndex * 2 + 1;
                int childIndexRight = _item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }
                    if (_item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(_item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        void SortUp(T _item)
        {
            int parentIndex = (_item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (_item.CompareTo(parentItem) > 0)
                {
                    Swap(_item, parentItem);
                }
                else
                {
                    break;
                }
                parentIndex = (_item.HeapIndex - 1) / 2;
            }
        }
        void Swap(T _itemA, T _itemB)
        {
            items[_itemA.HeapIndex] = _itemB;
            items[_itemB.HeapIndex] = _itemA;
            int itemAIndex = _itemA.HeapIndex;
            _itemA.HeapIndex = _itemB.HeapIndex;
            _itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }

    }
}
