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

        private readonly MouseEventListener _eventListener;

        private UIClickable(UIComponent component, IUIClickable handler, AreaType areaType)
        {
            _eventListener = MouseEventListener.Create();

            switch (areaType)
            {
                case AreaType.BOX:
                    Area2D = new RectangleArea2D
                    {
                        Update = area2d =>
                        {
                            area2d.Center = component.Position();
                            area2d.Rotation = component.Rotation();
                            area2d.Scale = component.GlobalScale();
                        }
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaType), areaType, null);
            }

            _eventListener.OnMouseDown += @event =>
            {
                var worldPos = mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos);
                if (Area2D.InArea(worldPos))
                    handler.MouseDown(worldPos);
            };

            _eventListener.OnMouseUp += @event =>
            {
                handler.MouseUp(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };

            _eventListener.OnMouseDrag += @event =>
            {
                handler.MouseDrag(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };
        }

        public static UIClickable Create(UIComponent component, IUIClickable handler, AreaType areaType)
        {
            return new UIClickable(component, handler, areaType);
        }
    }
}
