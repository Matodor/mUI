using System;

namespace mFramework.UI
{
    public class UIClickable
    {
        public Area2D Area2D;

        public event Func<IUIClickable, MouseEvent, bool> CanMouseDown = delegate { return true; };
        public event Func<IUIClickable, MouseEvent, bool> CanMouseUp = delegate { return true; };
        public event Func<IUIClickable, MouseEvent, bool> CanMouseDrag = delegate { return true; };

        private readonly IUIClickable _uiClickable;
        private readonly UIObject _uiObject;
        private readonly MouseEventListener _mouseEventListener;

        public UIClickable(IUIClickable clickable, Area2D area2d)
        {
            _uiClickable = clickable;
            _uiObject = clickable as UIObject;
            _uiObject.BeforeDestroy += OnBeforeDestroy;

            _mouseEventListener = MouseEventListener.Create();
            _mouseEventListener.MouseDown += MouseEventListenerOnMouseDown;
            _mouseEventListener.MouseUp += MouseEventListenerOnMouseUp;
            _mouseEventListener.MouseDrag += MouseEventListenerOnMouseDrag;

            Area2D = area2d;
            Area2D.Update += area =>
            {
                area.Center = _uiObject.Pos();
                area.Rotation = _uiObject.Rotation();
            };
        }

        private void MouseEventListenerOnMouseDrag(object sender, MouseEvent e)
        {
            if (!_uiObject.IsActive ||
                !CanMouseDrag(_uiClickable, e))
            {
                return;
            }

            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            _uiClickable.MouseDrag(worldPos);
        }

        private void MouseEventListenerOnMouseUp(object sender, MouseEvent e)
        {
            if (!_uiObject.IsActive ||
                !CanMouseUp(_uiClickable, e))
            {
                return;
            }

            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            _uiClickable.MouseUp(worldPos);
        }

        private void MouseEventListenerOnMouseDown(object sender, MouseEvent e)
        {
            if (!_uiObject.IsActive ||
                !CanMouseDown(_uiClickable, e))
            {
                return;
            }

            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            if (Area2D.InArea(worldPos))
                _uiClickable.MouseDown(worldPos);
        }

        private void OnBeforeDestroy(IUIObject sender)
        {
            _mouseEventListener.Detach();
        }
    }
}
