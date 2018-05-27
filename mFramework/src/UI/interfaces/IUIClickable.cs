using UnityEngine;

namespace mFramework.UI
{
    public delegate void UIMouseEvent(IUIClickable sender, Vector2 worldPos);
    public delegate bool UIMouseAllowEvent(IUIClickable sender, ref Vector2 worldPos);

    public interface IUIClickable : IUIObject
    {
        event UIMouseEvent MouseDown;
        event UIMouseEvent MouseUp;

        event UIMouseAllowEvent CanMouseDown;
        event UIMouseAllowEvent CanMouseUp;

        bool IgnoreByHandler { get; set; }
        bool IsPressed { get; }
        IAreaChecker AreaChecker { get; set; }

        void DoMouseDown(Vector2 worldPos);
        void DoMouseUp(Vector2 worldPos);
    }
}