using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public class ButtonEventArgs : EventArgs
    {
        public Vector2 ClickWorldPos { get; }

        public ButtonEventArgs(Vector2 worldPos)
        {
            ClickWorldPos = worldPos;
        }
    }
}
