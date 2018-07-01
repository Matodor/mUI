using UnityEngine;

namespace mFramework.UI
{
    public interface IUIDragable : IUIClickable
    {
        event UIMouseEvent MouseDrag;
        event UIMouseAllowEvent CanMouseDrag;

        void DoMouseDrag(Vector2 worldPos);
    }
}