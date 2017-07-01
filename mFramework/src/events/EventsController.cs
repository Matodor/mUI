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
            {

            }
            else
            {
                DesktopEvents();
            }
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
                            MouseWheelEvent(mouseEvent);
                            break;
                        case EventType.MouseDown:
                            mouseEvent.MouseEventType = MouseEventType.MouseDown;
                            MouseDownEvent(mouseEvent);
                            break;
                        case EventType.MouseUp:
                            mouseEvent.MouseEventType = MouseEventType.MouseUp;
                            MouseUpEvent(mouseEvent);
                            break;
                        case EventType.MouseDrag:
                            mouseEvent.MouseEventType = MouseEventType.MouseDrag;
                            MouseDragEvent(mouseEvent);
                            break;
                        default:
                            mouseEvent.MouseEventType = MouseEventType.NONE;
                            break;
                    }
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
