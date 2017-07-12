using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIClickable
    {
        public Area2D Area2D { get; }
        public event Func<IUIClickable, MouseEvent, bool> CanMouseDown;
        public event Func<IUIClickable, MouseEvent, bool> CanMouseUp;
        public event Func<IUIClickable, MouseEvent, bool> CanMouseDrag;

        private readonly UIObject _component;
        private readonly MouseEventListener _eventListener;

        private UIClickable(UIObject component, AreaType areaType)
        {
            _component = component;
            _eventListener = MouseEventListener.Create();
            _component.BeforeDestroy += sender => _eventListener.Detach();

            switch (areaType)
            {
                case AreaType.RECTANGLE:
                    Area2D = new RectangleArea2D();
                    Area2D.Update += area2d =>
                    {
                        area2d.Center = _component.Position();
                        area2d.Rotation = _component.Rotation();
                        area2d.Scale = _component.GlobalScale() * area2d.AdditionalScale;
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaType), areaType, null);
            }

            _eventListener.OnMouseDown += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseDown?.Invoke((IUIClickable)_component, e) ?? false))
                    return;

                var worldPos = WorldPos(e);
                if (Area2D.InArea(worldPos))
                    ((IUIClickable)_component)?.MouseDown(worldPos);
            };

            _eventListener.OnMouseUp += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseUp?.Invoke((IUIClickable)_component, e) ?? false))
                    return;
                ((IUIClickable)_component)?.MouseUp(WorldPos(e));
            };

            _eventListener.OnMouseDrag += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseDrag?.Invoke((IUIClickable)_component, e) ?? false))
                    return;
                ((IUIClickable)_component)?.MouseDrag(WorldPos(e));
            };
        }

        public bool InArea(Vector2 worldPos)
        {
            return Area2D.InArea(worldPos);
        }

        public static Vector2 WorldPos(MouseEvent e)
        {
            return mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
        }

        public static UIClickable Create(UIComponent component, AreaType areaType)
        {
            if (!(component is IUIClickable))
                throw new Exception("The given component not a IUIClickable");
            return new UIClickable(component, areaType);
        }
    }
}
