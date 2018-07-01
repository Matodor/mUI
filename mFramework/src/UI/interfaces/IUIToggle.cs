namespace mFramework.UI
{
    public delegate bool UIToggleAllowChangeState(IUIToggle sender);
    public delegate void UIToggleStateChangedEvent(IUIToggle sender);

    public interface IUIToggle : IUIClickable
    {
        bool IsSelected { get; }

        event UIToggleAllowChangeState CanSelect;
        event UIToggleAllowChangeState CanDeselect;

        event UIToggleStateChangedEvent Selected;
        event UIToggleStateChangedEvent Deselected;
        event UIToggleStateChangedEvent Changed;

        IUIToggle Toggle();
        IUIToggle Select();
        IUIToggle Deselect();
    }
}