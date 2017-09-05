using System;

namespace mFramework
{
    public class UnidirectionalList<T> where T : IGlobalUniqueIdentifier
    {
        public class ListItem
        {
            internal T Value;
            internal ListItem Prev;
        }

        public uint Count => _count;
        public ListItem LastItem => _lastItem;

        private ListItem _lastItem;
        private uint _count;

        private UnidirectionalList()
        {
            _count = 0;
            _lastItem = null;
        }

        public static UnidirectionalList<T> Create()
        {
            return new UnidirectionalList<T>();
        }

        public void Add(T value)
        {
            if (value == null)
                return;

            var item = new ListItem
            {
                Value = value,
                Prev = _lastItem
            };

            _lastItem = item;
            _count++;
        }

        public bool Remove(T value)
        {
            return value != null && Remove(value.GUID);
        }

        public bool Remove(ulong guid)
        {
            if (_lastItem == null)
                return false;

            ListItem lastIterated = null;
            var current = _lastItem;
            
            while (current != null)
            {
                if (current.Value.GUID == guid)
                {
                    if (lastIterated == null)
                        _lastItem = current.Prev;
                    else
                        lastIterated.Prev = current.Prev;

                    current.Value = default(T);
                    current.Prev = null;
                    current = null;
                    _count--;
                    return true;
                }

                lastIterated = current;
                current = current.Prev;
            }

            return false;
        }

        public void Clear()
        {
            ListItem lastIterated = null;
            var current = _lastItem;

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

            _lastItem = null;
            _count = 0;
        }

        public bool Contains(T item)
        {
            var current = _lastItem;
            while (current != null)
            {
                if (current.Value.GUID == item.GUID)
                    return true;
                current = current.Prev;
            }

            return false;
        }

        public void ForEach(Action<T> action)
        {
            var tmp = _lastItem;
            while (tmp != null)
            {
                action(tmp.Value);
                tmp = tmp.Prev;
            }
        }
    }
}
