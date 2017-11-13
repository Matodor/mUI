using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public class AddedAnimationEventArgs : EventArgs
    {
        public UIAnimation AddedAnimation { get; }

        public AddedAnimationEventArgs(UIAnimation addedAnimation)
        {
            AddedAnimation = addedAnimation;
        }
    }
}
