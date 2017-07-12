using System;

namespace mFramework.UI
{
    public class AnimationEventArgs : EventArgs
    {
        public UIAnimation Animation { get; }

        public AnimationEventArgs(UIAnimation animation)
        {
            Animation = animation;
        }
    }
}
