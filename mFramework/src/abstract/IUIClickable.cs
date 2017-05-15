using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public interface IUIClickable
    {
        void MouseDown(Vector2 worldPos);
        void MouseUp(Vector2 worldPos);
        void MouseDrag(Vector2 worldPos);
    }
}
