using UnityEngine;

namespace mFramework.UI
{
    public interface IUIClickable
    {
        UIClickable UIClickable { get; }
        void MouseDown(Vector2 worldPos);
        void MouseUp(Vector2 worldPos);
        void MouseDrag(Vector2 worldPos);
    }
}
