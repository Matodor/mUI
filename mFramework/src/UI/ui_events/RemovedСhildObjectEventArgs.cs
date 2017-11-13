using System;

namespace mFramework.UI
{
    public class RemovedСhildObjectEventArgs : EventArgs
    {
        public UIObject RemovedObject { get; }

        public RemovedСhildObjectEventArgs(UIObject removedObject)
        {
            RemovedObject = removedObject;
        }
    }
}
