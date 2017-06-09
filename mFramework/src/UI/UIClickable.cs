﻿using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIClickable
    {
        public Area2D Area2D { get; }
        public event Func<IUIClickable, MouseEvent, bool> CanMouseDown, CanMouseUp, CanMouseDrag;

        private readonly MouseEventListener _eventListener;

        private UIClickable(UIComponent component, IUIClickable handler, AreaType areaType)
        {
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
                if (!CanMouseDown?.Invoke(handler, @event) ?? false)
                    return;

                var worldPos = mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos);
                if (Area2D.InArea(worldPos))
                    handler.MouseDown(worldPos);
            };

            _eventListener.OnMouseUp += @event =>
            {
                if (!CanMouseUp?.Invoke(handler, @event) ?? false)
                    return;

                handler.MouseUp(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };

            _eventListener.OnMouseDrag += @event =>
            {
                if (!CanMouseDrag?.Invoke(handler, @event) ?? false)
                    return;

                handler.MouseDrag(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos));
            };
        }

        public bool InArea(Vector2 worldPos)
        {
            return Area2D.InArea(worldPos);
        }

        public Vector2 WorldPos(MouseEvent @event)
        {
            return mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos);
        }

        public static UIClickable Create(UIComponent component, IUIClickable handler, AreaType areaType)
        {
            return new UIClickable(component, handler, areaType);
        }
    }
}
