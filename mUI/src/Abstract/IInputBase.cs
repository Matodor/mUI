using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Input
{
    public interface IInputBase
    {
        event Action<mUIMouseEvent> OnMouseDownEvent;
        event Action<mUIMouseEvent> OnMouseUpEvent;
        event Action<mUIMouseEvent> OnMouseDragEvent;
        event Action<mUIKeyboardEvent> OnKeyUpEvent;
        event Action<mUIKeyboardEvent> OnKeyDownEvent;

        void ParseEvent(mGUIEvent @event);
        void OnGUI();
        void Update();
    }

    public enum mUIEventType
    {
        NONE = 0,
        MOUSE_EVENT = 1,
        KEYBOARD_EVENT = 2,
    }

    public enum mUIKeyboardEventType
    {
        NONE = 0,
        KeyUp = 1,
        KeyDown = 2,
    }

    public enum mUIMouseEventType
    {
        NONE = 0,
        MouseDown = 1,
        MouseUp = 2,
        MouseDrag = 4,
        ScrollWheel = 5,
    }

    public abstract class mGUIEvent
    {
        public mUIEventType Type { get; protected set; }
    }

    public class mUIKeyboardEvent : mGUIEvent
    {
        public mUIKeyboardEvent()
        {
            Type = mUIEventType.KEYBOARD_EVENT;
        }

        public mUIKeyboardEventType KeyboardEventType { get; set; }
        public KeyCode KeyCode;
        public EventModifiers Modifiers;
        public bool Alt;
        public bool FunctionKey;
        public bool Numeric;
        public bool Shift;
        public bool Control;
        public bool CapsLock;
        public bool Command;
        public string CommandName;
        public char Character;
    }

    public class mUIMouseEvent : mGUIEvent
    {
        public mUIMouseEvent()
        {
            Type = mUIEventType.MOUSE_EVENT;
        }

        public mUIMouseEventType MouseEventType;
        public KeyCode KeyCode;
        public Vector2 MouseScreenPos;
        public Vector2 Delta;
        public int Button;
        public int ClickCount;
    }
}
