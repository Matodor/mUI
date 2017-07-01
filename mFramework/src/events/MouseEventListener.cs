using System;

namespace mFramework
{
    public class MouseEventListener : EventListener
    {
        public event Action<MouseEvent> OnMouseDown;
        public event Action<MouseEvent> OnMouseUp;
        public event Action<MouseEvent> OnMouseDrag;
        public event Action<MouseEvent> OnMouseWheel;

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

        public override void Detach()
        {
            EventsController.RemoveMouseEventListener(this);
        }
    }
}
