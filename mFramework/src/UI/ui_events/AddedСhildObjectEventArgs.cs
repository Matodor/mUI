using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public class AddedСhildObjectEventArgs : EventArgs
    {
        public UIObject AddedObject { get; }

        public AddedСhildObjectEventArgs(UIObject addedObject)
        {
            AddedObject = addedObject;
        }
    }
}
