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

        private readonly IUIClickable _uiClickable;
        private readonly UIObject _component;
        private readonly MouseEventListener _eventListener;

        private UIClickable(IUIClickable clickable, AreaType areaType)
        {
            _uiClickable = clickable;
            _component = clickable as UIObject;
            if (_component == null)
                throw new Exception("The given \"clickable\" parameter is not UIObject");
            
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

            _eventListener.MouseDown += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseDown?.Invoke(_uiClickable, e) ?? false))
                    return;

                var worldPos = WorldPos(e);
                if (Area2D.InArea(worldPos))
                    _uiClickable?.MouseDown(worldPos);
            };

            _eventListener.MouseUp += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseUp?.Invoke(_uiClickable, e) ?? false))
                    return;
                _uiClickable?.MouseUp(WorldPos(e));
            };

            _eventListener.MouseDrag += (s, e) =>
            {
                if (!_component.IsActive || (!CanMouseDrag?.Invoke(_uiClickable, e) ?? false))
                    return;
                _uiClickable?.MouseDrag(WorldPos(e));
            };
        }

        public bool InArea(MouseEvent e)
        {
            return Area2D.InArea(WorldPos(e));
        }
        
        public bool InArea(Vector2 worldPos)
        {
            return Area2D.InArea(worldPos);
        }

        public static Vector2 WorldPos(MouseEvent e)
        {
            return mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
        }

        public static UIClickable Create(IUIClickable clickable, AreaType areaType)
        {
            return new UIClickable(clickable, areaType);
        }
    }
}
