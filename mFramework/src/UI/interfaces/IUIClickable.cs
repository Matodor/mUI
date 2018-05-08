using UnityEngine;

namespace mFramework.UI
{
    public interface IUIClickable : IUIObject
    {
        IAreaChecker AreaChecker { get; set; }

        void MouseDown(Vector2 worldPos);
        void MouseUp(Vector2 worldPos);
    }
}