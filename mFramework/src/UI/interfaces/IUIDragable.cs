using UnityEngine;

namespace mFramework.UI
{
    public interface IUIDragable : IUIClickable
    {
        void MouseDrag(Vector2 worldPos);
    }
}