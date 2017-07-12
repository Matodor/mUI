using System;

namespace mFramework.UI
{
    public class RemovedAnimationEventArgs : EventArgs
    {
        public UIAnimation RemovedAnimation { get; }

        public RemovedAnimationEventArgs(UIAnimation removedAnimation)
        {
            RemovedAnimation = removedAnimation;
        }
    }
}
