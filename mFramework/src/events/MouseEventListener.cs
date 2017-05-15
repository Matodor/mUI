﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public class MouseEventListener : EventListener
    {
        public event Action<MouseEvent> OnMouseDown;
        public event Action<MouseEvent> OnMouseUp;
        public event Action<MouseEvent> OnMouseDrag;
        public event Action<MouseEvent> OnMouseWheel;

        public static MouseEventListener Create()
        {
            var listener = new MouseEventListener();
            EventsController.AddMouseEventListener(listener);
            return listener;
        }

        public void MouseWheel(MouseEvent @event)
        {
            OnMouseWheel?.Invoke(@event);
        }

        public void MouseDrag(MouseEvent @event)
        {
            OnMouseDrag?.Invoke(@event);
        }

        public void MouseDown(MouseEvent @event)
        {
            OnMouseDown?.Invoke(@event);
        }

        public void MouseUp(MouseEvent @event)
        {
            OnMouseUp?.Invoke(@event);
        }
    }
}