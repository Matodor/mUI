using System;
using System.Collections;
using System.Collections.Generic;

namespace mFramework
{
    public class UnidirectionalList<T> where T : IGlobalUniqueIdentifier
    {
        public class ListItem
        {
            public T Value { get; internal set; }
            public ListItem Prev { get; internal set; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    return default(T);

                var tmp = LastItem;
                var i = 0;

                while (tmp != null)
                {
                    if (i == index)
                        return tmp.Value;
                    tmp = tmp.Prev;
                    i++;
                }

                return default(T);
            }
        }
        
        public int Count { get; private set; }
        public ListItem LastItem { get; private set; }
        public ListItem FirstItem { get; private set; }

        private UnidirectionalList()
        {
            Count = 0;
            LastItem = null;
            FirstItem = null;
        }

        public static UnidirectionalList<T> Create()
        {
            return new UnidirectionalList<T>();
        }

        internal void Add(T value)
        {
            if (value == null)
                return;

            var item = new ListItem
            {
                Value = value,
                Prev = LastItem
            };

            if (FirstItem == null)
                FirstItem = item;

            LastItem = item;
            Count++;
        }

        internal bool Remove(T value)
        {
            return value != null && Remove(value.GUID);
        }

        internal bool Remove(ulong guid)
        {
            if (Count == 0)
                return false;

            var lastIterated = (ListItem) null;
            var current = LastItem;
            
            while (current != null)
            {
                if (current.Value.GUID == guid)
                {
                    if (lastIterated == null)
                        LastItem = current.Prev;
                    else
                        lastIterated.Prev = current.Prev;

                    current.Value = default(T);
                    current.Prev = null;
                    current = null;
                    Count--;

                    if (Count == 0)
                        FirstItem = null;

                    return true;
                }

                lastIterated = current;
                current = current.Prev;
            }

            return false;
        }

        internal void Clear()
        {
            ListItem lastIterated = null;
            var current = LastItem;

            while (current != null)
            {
                if (lastIterated != null)
                {
                    lastIterated.Prev = null;
                    lastIterated.Value = default(T);
                }

                lastIterated = current;
                current = current.Prev;
            }

            FirstItem = null;
            LastItem = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            var current = LastItem;
            while (current != null)
            {
                if (current.Value.GUID == item.GUID)
                    return true;
                current = current.Prev;
            }

            return false;
        }

        public T Find(Func<T, bool> predicate)
        {
            var tmp = LastItem;
            while (tmp != null)
            {
                if (predicate(tmp.Value))
                    return tmp.Value;
                tmp = tmp.Prev;
            }

            return default(T);
        }

        public void ForEach(Action<T> action)
        {
            var tmp = LastItem;
            while (tmp != null)
            {
                action(tmp.Value);
                tmp = tmp.Prev;
            }
        }
    }
}
