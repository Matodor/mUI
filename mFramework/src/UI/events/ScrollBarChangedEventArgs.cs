using System;

namespace mFramework.UI
{
    public class ScrollBarChangedEventArgs : EventArgs
    {
        public float NewValue { get; }
        public float OldValue { get; }

        public ScrollBarChangedEventArgs(float newValue, float oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
