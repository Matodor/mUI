using System;
using UnityEngine;

namespace mFramework.UI
{
    public class RotatedEventArgs : EventArgs
    {
        public Vector3 PreviousAngle { get; }
        public Vector3 NewAngle { get; }

        public RotatedEventArgs(Vector3 previousAngle, Vector3 newAngle)
        {
            PreviousAngle = previousAngle;
            NewAngle = newAngle;
        }
    }
}
