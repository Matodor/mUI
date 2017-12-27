using System;
using UnityEngine;

namespace mFramework.UI
{
    public interface IUIButton : IUIClickable
    {
        event UIEventHandler<IUIButton> Click;
        event Func<IUIButton, Vector2, bool> CanButtonClick;

        event UIEventHandler<IUIButton, Vector2> ButtonDown;
        event UIEventHandler<IUIButton, Vector2> ButtonUp;
    }
}