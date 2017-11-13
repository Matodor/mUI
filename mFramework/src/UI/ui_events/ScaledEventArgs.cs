using System;
using UnityEngine;

namespace mFramework.UI
{
    public class ScaledEventArgs : EventArgs
    {
        public Vector3 PreviousScale { get; }
        public Vector3 NewScale { get; }

        public ScaledEventArgs(Vector3 previousScale, Vector3 newScale)
        {
            PreviousScale = previousScale;
            NewScale = newScale;
        }
    }
}
