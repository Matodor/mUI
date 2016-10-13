using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Input
{
    class mGUIMobileGetEvents : IInputGetEvents
    {
        public void GetEvents(IInputBase input)
        {
            if (UnityEngine.Input.touchCount == 0)
                return;

            mUIMouseEvent mouseEvent = new mUIMouseEvent();
            for (var i = 0; i < UnityEngine.Input.touchCount; ++i)
            {
                var t = UnityEngine.Input.GetTouch(i);
                mouseEvent.Button = t.fingerId;
                mouseEvent.Delta = t.deltaPosition;
                mouseEvent.ClickCount = t.tapCount;
                mouseEvent.MouseScreenPos = t.position;
                mouseEvent.KeyCode = KeyCode.Mouse0;

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseDown;
                        break;
                    case TouchPhase.Moved:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseDrag;
                        break;
                    case TouchPhase.Stationary:
                        mouseEvent.MouseEventType = mUIMouseEventType.NONE;
                        break;
                    case TouchPhase.Ended:
                        mouseEvent.MouseEventType = mUIMouseEventType.MouseUp;
                        break;
                    case TouchPhase.Canceled:
                        mouseEvent.MouseEventType = mUIMouseEventType.NONE;
                        break;
                }

                input.ParseEvent(mouseEvent);
            }
        }
    }
}
