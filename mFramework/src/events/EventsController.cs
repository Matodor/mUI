using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace mFramework
{
    internal class EventsController
    {
        public static EventsController Instance { get; }

        private readonly Event _currentEvent;
        private readonly UnidirectionalList<MouseEventListener> _mouseEventListeners;

        static EventsController()
        {
            Instance = new EventsController();
        }

        private EventsController()
        {
            _currentEvent = new Event();
            _mouseEventListeners = UnidirectionalList<MouseEventListener>.Create();
        }

        public static bool RemoveMouseEventListener(MouseEventListener listener)
        {
            return Instance._mouseEventListeners.Remove(listener.GUID);
        }

        public static MouseEventListener AddMouseEventListener(MouseEventListener listener)
        {
            if (!Instance._mouseEventListeners.Contains(listener))
            {
                Instance._mouseEventListeners.Add(listener);
                return listener;
            }
            return null;
        }

        public void Update()
        {
            if (Application.isMobilePlatform)
                MobileEvents();
            else
                DesktopEvents();
        }

        private void MouseWheelEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.MouseWheel(@event));
        }

        private void MouseDragEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.MouseDrag(@event));
        }

        private void MouseUpEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.MouseUp(@event));
        }

        private void MouseDownEvent(MouseEvent @event)
        {
            _mouseEventListeners.ForEach(listener => listener.MouseDown(@event));
        }

        private void MouseEvent(MouseEvent mouseEvent)
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

        private void MobileEvents()
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
                    KeyCode = KeyCode.Mouse0
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
                }

                MouseEvent(touchEvent);
            }
        }

        private void DesktopEvents()
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
