using UnityEngine;

namespace mFramework.UI
{
    public interface IUIClickable : IUIObject
    {
        UIClickableOld UiClickableOld { get; }

        void MouseDown(Vector2 worldPos);
        void MouseUp(Vector2 worldPos);
        void MouseDrag(Vector2 worldPos);
    }
}