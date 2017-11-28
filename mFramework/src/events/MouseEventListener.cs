using System;

namespace mFramework
{
    public class MouseEventListener : EventListener
    {
        public bool Enabled;

        public event EventHandler<MouseEvent> MouseDown = delegate { };
        public event EventHandler<MouseEvent> MouseUp = delegate { };
        public event EventHandler<MouseEvent> MouseDrag = delegate { };
        public event EventHandler<MouseEvent> MouseWheel = delegate { };

        private bool _detached;

        private MouseEventListener()
        {
            Enabled = true;
            _detached = false;
        }

        public static MouseEventListener Create()
        {
            return EventsController.AddMouseEventListener(new MouseEventListener());
        }

        ~MouseEventListener()
        {
            if (!_detached)
                Detach();
        }

        internal void OnMouseWheel(MouseEvent @event)
        {
            if (Enabled)
                MouseWheel.Invoke(this, @event);
        }

        internal void OnMouseDrag(MouseEvent @event)
        {
            if (Enabled)
                MouseDrag.Invoke(this, @event);
        }

        internal void OnMouseDown(MouseEvent @event)
        {
            if (Enabled)
                MouseDown?.Invoke(this, @event);
        }

        internal void OnMouseUp(MouseEvent @event)
        {
            if (Enabled) MouseUp.Invoke(this, @event);
        }

        public override void Detach()
        {
            _detached = true;
            MouseDown = null;
            MouseUp = null;
            MouseDrag = null;
            MouseWheel = null;
            EventsController.RemoveMouseEventListener(this);
        }
    }
}
