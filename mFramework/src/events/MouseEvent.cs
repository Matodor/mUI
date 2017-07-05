using UnityEngine;

namespace mFramework
{
    public enum MouseEventType
    {
        NONE = 0,
        MouseDown = 1,
        MouseUp = 2,
        MouseDrag = 4,
        ScrollWheel = 5,
    }

    public class MouseEvent : InputEvent
    {
        public MouseEventType MouseEventType { get; set; }
        public KeyCode KeyCode { get; set; }
        public Vector2 MouseScreenPos { get; set; }
        public Vector2 Delta { get; set; }
        public int Button { get; set; }
        public int ClickCount { get; set; }

        public override string ToString()
        {
            return MouseEventType.ToString();
        }
    }
}
