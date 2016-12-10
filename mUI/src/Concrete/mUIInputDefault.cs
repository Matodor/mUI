using System;
using UnityEngine;

namespace mUIApp.Input
{
    public class mUIInputDefault : IInputBase
    {
        public event Action<mUIMouseEvent> OnMouseDownEvent;
        public event Action<mUIMouseEvent> OnMouseUpEvent;
        public event Action<mUIMouseEvent> OnMouseDragEvent;
        public event Action<mUIKeyboardEvent> OnKeyUpEvent;
        public event Action<mUIKeyboardEvent> OnKeyDownEvent;

        private IInputGetEvents _getEventsController;
        private Action<IInputBase> _onGUI, _onUpdate;

        public mUIInputDefault()
        {
            OnMouseDownEvent = i => { };
            OnMouseUpEvent = i => { };
            OnMouseDragEvent = i => { };
            OnKeyUpEvent = i => { };
            OnKeyDownEvent = i => { };

            _onGUI = i => { };
            _onUpdate = i => { };

            switch (Application.platform)
            {
                case RuntimePlatform.Android: SetupMobile(); break;
                case RuntimePlatform.IPhonePlayer: SetupMobile(); break;
                case RuntimePlatform.WindowsEditor: SetupDesktop(); break;
                case RuntimePlatform.WindowsPlayer: SetupDesktop(); break;
            }
        }

        private void SetupMobile()
        {
            _getEventsController = new mUIMobileGetEvents();
            _onUpdate = _getEventsController.GetEvents;
        }

        private void SetupDesktop()
        {
            _getEventsController = new mUIDesktopGetEvents();
            _onGUI = _getEventsController.GetEvents;
        }

        public void ParseEvent(mUIEvent @event)
        {
            if (@event.Type == mUIEventType.MOUSE_EVENT)
            {
                mUIMouseEvent mouseEvent = (mUIMouseEvent) @event;
                switch (mouseEvent.MouseEventType)
                {
                    case mUIMouseEventType.NONE:
                        break;
                    case mUIMouseEventType.MouseDown:
                        OnMouseDownEvent(mouseEvent);
                        break;
                    case mUIMouseEventType.MouseUp:
                        OnMouseUpEvent(mouseEvent);
                        break;
                    case mUIMouseEventType.MouseDrag:
                        OnMouseDragEvent(mouseEvent);
                        break;
                    case mUIMouseEventType.ScrollWheel:
                        break;
                }
            }
            else if (@event.Type == mUIEventType.KEYBOARD_EVENT)
            {
                mUIKeyboardEvent keyboardEvent = (mUIKeyboardEvent) @event;
                switch (keyboardEvent.KeyboardEventType)
                {
                    case mUIKeyboardEventType.KeyUp:
                        OnKeyUpEvent(keyboardEvent);
                        break;
                    case mUIKeyboardEventType.KeyDown:
                        OnKeyDownEvent(keyboardEvent);
                        break;
                }
            }
        }

        public void OnGUI()
        {
            _onGUI(this);
        }

        public void Update()
        {
            _onUpdate(this);
        }
    }
}
