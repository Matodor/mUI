using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mFramework.UI;

namespace mFramework.src.UI
{
    public class UIClickable
    {
        public Area2D Area2D { get; }
        public Func<bool> CanClick { get; set; }

        private readonly MouseEventListener _eventListener;

        private UIClickable(UIComponent component, IUIClickable handler, AreaType areaType)
        {
            CanClick = () => true;
            _eventListener = MouseEventListener.Create();

            switch (areaType)
            {
                case AreaType.RECTANGLE:
                    Area2D = new RectangleArea2D();
                    Area2D.Update += area2d =>
                    {
                        area2d.Center = component.Position();
                        area2d.Rotation = component.Rotation();
                        area2d.Scale = component.GlobalScale();
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaType), areaType, null);
            }

            _eventListener.OnMouseDown += @event =>
            {
                if (!Can()) return;

                var worldPos = mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos);
                if (Area2D.InArea(worldPos))
                    handler.MouseDown(worldPos);
            };

            _eventListener.OnMouseUp += @event =>
            {
                if (!Can()) return;

                handler.MouseUp(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };

            _eventListener.OnMouseDrag += @event =>
            {
                if (!Can()) return;

                handler.MouseDrag(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };
        }

        private bool Can()
        {
            return CanClick != null && CanClick();
        }

        public static UIClickable Create(UIComponent component, IUIClickable handler, AreaType areaType)
        {
            return new UIClickable(component, handler, areaType);
        }
    }
}
