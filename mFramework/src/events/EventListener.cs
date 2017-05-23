using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public abstract class EventListener
    {
        internal long GUID { get; }

        private static long _guid = 0;

        protected EventListener()
        {
            GUID = ++_guid;
        }

        public abstract void Detach();
    }
}
