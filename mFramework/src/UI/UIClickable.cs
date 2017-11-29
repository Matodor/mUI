using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIClickable
    {
        public Area2D Area2D { get; set; }

        public event Func<IUIClickable, MouseEvent, bool> CanMouseDown = delegate { return true; };
        public event Func<IUIClickable, MouseEvent, bool> CanMouseUp = delegate { return true; };
        public event Func<IUIClickable, MouseEvent, bool> CanMouseDrag = delegate { return true; };

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
                    var area = new RectangleArea2D();
                    Area2D = area;
                    Area2D.Update += _ =>
                    {
                        area.Center = _component.Position();
                        area.Rotation = _component.Rotation();
                        area.Width = _component.GetWidth();
                        area.Height = _component.GetHeight();
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaType), areaType, null);
            }

            _eventListener.MouseDown += (s, e) =>
            {
                if (!_component.IsActive || !CanMouseDown.Invoke(_uiClickable, e))
                    return;

                var worldPos = WorldPos(e);
                if (Area2D.InArea(worldPos))
                    _uiClickable?.MouseDown(worldPos);
            };

            _eventListener.MouseUp += (s, e) =>
            {
                if (!_component.IsActive || !CanMouseUp.Invoke(_uiClickable, e))
                    return;
                _uiClickable?.MouseUp(WorldPos(e));
            };

            _eventListener.MouseDrag += (s, e) =>
            {
                if (!_component.IsActive || !CanMouseDrag.Invoke(_uiClickable, e))
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

        public static UIClickable Create<T>(T clickable, AreaType areaType) where T : IUIClickable
        {
            return new UIClickable(clickable, areaType);
        }
    }
}
