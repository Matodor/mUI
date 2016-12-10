using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Input
{
    public class mUIDesktopGetEvents : IInputGetEvents
    {
        public void GetEvents(IInputBase input)
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.Repaint || currentEvent.type == EventType.Layout || currentEvent.type == EventType.Ignore)
                return;

            if (currentEvent.isMouse)
            {
                mUIMouseEvent mouseEvent = new mUIMouseEvent
                {
                    Button = currentEvent.button,
                    ClickCount = currentEvent.clickCount,
                    Delta = currentEvent.delta,
                    MouseScreenPos = currentEvent.mousePosition,
                    KeyCode = currentEvent.keyCode
                };

                switch (currentEvent.type)
                {
                    case EventType.ScrollWheel:
                        mouseEvent.MouseEventType = mUIMouseEventType.ScrollWheel;
                        break;
                    case EventType.MouseDown:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseDown;
                        break;
                    case EventType.MouseUp:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseUp;
                        break;
                    case EventType.MouseDrag:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseDrag;
                        break;
                    default:
                        mouseEvent.MouseEventType = mUIMouseEventType.NONE;
                        break;
                }
                input.ParseEvent(mouseEvent);
                return;
            }

            if (currentEvent.isKey)
            {
                mUIKeyboardEvent keyboardEvent = new mUIKeyboardEvent
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

                input.ParseEvent(keyboardEvent);
            }
        }
    }
}
