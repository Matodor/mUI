using System;
using UnityEngine;

namespace mFramework.UI
{
    public class TranslateEventArgs : EventArgs
    {
        public Vector3 Translate { get; }
        public Vector3 NewPosition { get; }

        public TranslateEventArgs(Vector3 translate, Vector3 newPosition)
        {
            NewPosition = newPosition;
            Translate = translate;
        }
    }
}
