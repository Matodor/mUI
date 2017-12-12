using UnityEngine;

namespace mFramework
{
    internal static class EventsController
    {
        private static readonly Event _currentEvent;
        private static readonly UnidirectionalList<MouseEventListener> _mouseEventListeners;

        static EventsController()
        {
            _currentEvent = new Event();
            _mouseEventListeners = UnidirectionalList<MouseEventListener>.Create();
        }

        public static bool RemoveMouseEventListener(MouseEventListener listener)
        {
            return _mouseEventListeners.Remove(listener.GUID);
        }

        public static MouseEventListener AddMouseEventListener(MouseEventListener listener)
        {
            if (!_mouseEventListeners.Contains(listener))
            {
                _mouseEventListeners.Add(listener);
                return listener;
            }
            return null;
        }

        public static void Update()
        {
            if (Application.isMobilePlatform)
                MobileEvents();
            else
                DesktopEvents();
        }

        private static void MouseWheelEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.OnMouseWheel(@event));
        }

        private static void MouseDragEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.OnMouseDrag(@event));
        }

        private static void MouseUpEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.OnMouseUp(@event));
        }

        private static void MouseDownEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.OnMouseDown(@event));
        }

        private static void MouseEvent(MouseEvent mouseEvent)
        {
            switch (mouseEvent.MouseEventType)
            {
                case MouseEventType.ScrollWheel:
                    MouseWheelEvent(mouseEvent);
                    break;
                case MouseEventType.MouseDown:
                    MouseDownEvent(mouseEvent);
                    break;
                case MouseEventType.MouseUp:
                    MouseUpEvent(mouseEvent);
                    break;
                case MouseEventType.MouseDrag:
                    MouseDragEvent(mouseEvent);
                    break;
            }
        }

        private static void MobileEvents()
        {
            if (UnityEngine.Input.touchCount == 0)
                return;

            for (var i = 0; i < UnityEngine.Input.touchCount; ++i)
            {
                var touch = UnityEngine.Input.GetTouch(i);
                var touchEvent = new TouchEvent
                {
                    Button = touch.fingerId,
                    ClickCount = touch.tapCount,
                    Delta = touch.deltaPosition,
                    MouseScreenPos = touch.position,
                    KeyCode = KeyCode.Mouse0, 
                    Touch = touch
                };
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchEvent.MouseEventType = MouseEventType.MouseDown;
                        break;
                    case TouchPhase.Moved:
                        touchEvent.MouseEventType = MouseEventType.MouseDrag;
                        break;
                    case TouchPhase.Stationary:
                        touchEvent.MouseEventType = MouseEventType.NONE;
                        break;
                    case TouchPhase.Ended:
                        touchEvent.MouseEventType = MouseEventType.MouseUp;
                        break;
                    case TouchPhase.Canceled:
                        touchEvent.MouseEventType = MouseEventType.NONE;
                        break;
                    default:
                        touchEvent.MouseEventType = MouseEventType.NONE;
                        break;
                }

                MouseEvent(touchEvent);
            }
        }

        private static void DesktopEvents()
        {
            while (Event.PopEvent(_currentEvent))
            {
                if (_currentEvent.isMouse)
                {
                    var mouseEvent = new MouseEvent
                    {
                        Button = _currentEvent.button,
                        ClickCount = _currentEvent.clickCount,
                        Delta = _currentEvent.delta,
                        MouseScreenPos = _currentEvent.mousePosition,
                        KeyCode = _currentEvent.keyCode
                    };

                    switch (_currentEvent.type)
                    {
                        case EventType.ScrollWheel:
                            mouseEvent.MouseEventType = MouseEventType.ScrollWheel;
                            break;
                        case EventType.MouseDown:
                            mouseEvent.MouseEventType = MouseEventType.MouseDown;
                            break;
                        case EventType.MouseUp:
                            mouseEvent.MouseEventType = MouseEventType.MouseUp;
                            break;
                        case EventType.MouseDrag:
                            mouseEvent.MouseEventType = MouseEventType.MouseDrag;
                            break;
                        default:
                            mouseEvent.MouseEventType = MouseEventType.NONE;
                            break;
                    }

                    MouseEvent(mouseEvent);
                }
                else if (_currentEvent.isKey)
                {
                    /*mUIKeyboardEvent keyboardEvent = new mUIKeyboardEvent
                    {
                        KeyCode = currentEvent.keyCode,
                        Alt = currentEvent.alt,
                        CapsLock = currentEvent.capsLock,
                        Character = currentEvent.character,
                        Command = currentEvent.command,
                        CommandName = currentEvent.commandName,
                        Control = currentEvent.control,
                        FunctionKey = currentEvent.functionKey,
                        Modifiers = currentEvent.modifiers,
                        Numeric = currentEvent.numeric,
                        Shift = currentEvent.shift
                    };

                    switch (currentEvent.type)
                    {
                        case EventType.KeyDown:
                            keyboardEvent.KeyboardEventType = mUIKeyboardEventType.KeyDown;
                            break;
                        case EventType.KeyUp:
                            keyboardEvent.KeyboardEventType = mUIKeyboardEventType.KeyUp;
                            break;
                    }

                    input.ParseEvent(keyboardEvent);*/
                }
            }
        }
    }
}
