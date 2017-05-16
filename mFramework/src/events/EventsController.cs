using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace mFramework
{
    public class EventsController
    {
        public static EventsController Instance { get; }

        private readonly Event _currentEvent;
        private readonly Dictionary<long, MouseEventListener> _mouseEventListeners;

        static EventsController()
        {
            Instance = new EventsController();
        }

        private EventsController()
        {
            _currentEvent = new Event();
            _mouseEventListeners = new Dictionary<long, MouseEventListener>();
        }

        public static void RemoveMouseEventListener(MouseEventListener listener)
        {
            if (Instance._mouseEventListeners.ContainsKey(listener.GUID))
                Instance._mouseEventListeners.Remove(listener.GUID);
        }

        public static MouseEventListener AddMouseEventListener(MouseEventListener listener)
        {
            if (!Instance._mouseEventListeners.ContainsKey(listener.GUID))
            {
                Instance._mouseEventListeners.Add(listener.GUID, listener);
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
            foreach (var listener in _mouseEventListeners.Values)
            {
                listener.MouseWheel(@event);
            }
        }

        private void MouseDragEvent(MouseEvent @event)
        {
            foreach (var listener in _mouseEventListeners.Values)
            {
                listener.MouseDrag(@event);
            }
        }

        private void MouseUpEvent(MouseEvent @event)
        {
            foreach (var listener in _mouseEventListeners.Values)
            {
                listener.MouseUp(@event);
            }
        }

        private void MouseDownEvent(MouseEvent @event)
        {
            foreach (var listener in _mouseEventListeners.Values)
            {
                listener.MouseDown(@event);
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
