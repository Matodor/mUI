using System;

namespace mFramework
{
    public class MouseEventListener : EventListener
    {
        public event EventHandler<MouseEvent> OnMouseDown;
        public event EventHandler<MouseEvent> OnMouseUp;
        public event EventHandler<MouseEvent> OnMouseDrag;
        public event EventHandler<MouseEvent> OnMouseWheel;

        private MouseEventListener()
        {
        }

        public static MouseEventListener Create()
        {
            return EventsController.AddMouseEventListener(new MouseEventListener());
        }

        ~MouseEventListener()
        {
            Detach();
        }

        internal void MouseWheel(MouseEvent @event)
        {
            OnMouseWheel?.Invoke(this, @event);
        }

        internal void MouseDrag(MouseEvent @event)
        {
            OnMouseDrag?.Invoke(this, @event);
        }

        internal void MouseDown(MouseEvent @event)
        {
            OnMouseDown?.Invoke(this, @event);
        }

        internal void MouseUp(MouseEvent @event)
        {
            OnMouseUp?.Invoke(this, @event);
        }

        public override void Detach()
        {
            OnMouseDown = null;
            OnMouseUp = null;
            OnMouseDrag = null;
            OnMouseWheel = null;
            EventsController.RemoveMouseEventListener(this);
        }
    }
}
