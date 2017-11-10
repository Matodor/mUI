using System;
using System.Collections.Generic;

namespace mFramework
{
    public class ObjectPool<T> where T : class, IGlobalUniqueIdentifier
    {
        public class ListItem
        {
            public readonly T Object;
            public bool InUsage;

            internal ListItem(T obj, bool inUsage)
            {
                Object = obj;
                InUsage = inUsage;
            }
        }

        public int Count => _list.Count;

        private readonly Dictionary<ulong, ListItem> _list;

        public ObjectPool()
        {
            _list = new Dictionary<ulong, ListItem>();
        }

        public void Clear(Action<T> onClearAction)
        {
            if (onClearAction != null)
            {
                foreach (var kvp in _list)
                {
                    onClearAction(kvp.Value.Object);
                }
            }

            _list.Clear();
        }

        public void Add(T obj, bool inUsage)
        {
            _list.Add(obj.GUID, new ListItem(obj, inUsage));    
        }

        public void Release(T obj)
        {
            if (_list.ContainsKey(obj.GUID))
                _list[obj.GUID].InUsage = false;
        }

        public bool Remove(T obj)
        {
            return _list.Remove(obj.GUID);
        }

        public bool TryGet(out T obj)
        {
            foreach (var kvp in _list)
            {
                if (kvp.Value.InUsage)
                    continue;

                kvp.Value.InUsage = true;
                obj = kvp.Value.Object;
                return true;
            }

            obj = null;
            return false;
        }
    }
}