using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISliderSettings : UIComponentSettings
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Offset { get; set; }
        public UISliderOrientationSettings OrientationSettings { get; set; }
    }

    public abstract class UISliderOrientationSettings
    {
    }

    public sealed class UISliderVerticalSettings : UISliderOrientationSettings
    {
        public enum Direction
        {
            TOP_TO_BOTTOM = 0,
            BOTTOM_TO_TOP = 1,
        }

        public Direction SliderDirection { get; set; }
    }

    public sealed class UISliderHorizontalSettings : UISliderOrientationSettings
    {
        public enum Direction
        {
            LEFT_TO_RIGHT = 0,
            RIGHT_TO_LEFT = 1,
        }

        public Direction SliderDirection { get; set; }
    }

    public class UISlider : UIComponent, IUIClickable
    {
        public UIClickable UIClickable { get { return _clickableHandler; } }

        private readonly List<UIObject> _slides;
        private UIClickable _clickableHandler;
        private float _height;
        private float _width;
        private UISliderOrientationSettings _sliderOrientation;
        private bool _isPressed;
        private Vector2 _lastMousePos;
        private float _offset;

        private UISlider(UIObject parent) : base(parent)
        {
            _isPressed = false;
            _slides = new List<UIObject>();
            OnAddedChildren += SetupChildren;
        }

        private void SetupChildren(UIObject obj)
        {
            var horizontal = _sliderOrientation as UISliderHorizontalSettings;
            if (horizontal != null)
                SetupChildrenHorizontal(obj, horizontal);
            
            var vertical = _sliderOrientation as UISliderVerticalSettings;
            if (vertical != null)
                SetupChildrenVertical(obj, vertical);

            _slides.Add(obj);
            OnRecursiveAddedChildren(obj);
        }

        private void OnRecursiveAddedChildren(UIObject uiObject)
        {
            var uiClickable = uiObject as IUIClickable;
            if (uiClickable != null)
                uiClickable.UIClickable.CanClick += CanChildsClick;

            foreach (var child in uiObject.ChildsObjects)
                OnRecursiveAddedChildren(child);
            uiObject.OnAddedChildren += OnRecursiveAddedChildren;
        }

        private bool CanChildsClick()
        {
            return false;
        }

        private void SetupChildrenHorizontal(UIObject obj, UISliderHorizontalSettings settings)
        {
            var rect = GetRect();

            switch (settings.SliderDirection)
            {
                case UISliderHorizontalSettings.Direction.LEFT_TO_RIGHT:
                    mCore.Log(new Vector2(rect.Left + obj.GetWidth() / 2, rect.Position.y).ToString());
                    if (_slides.Count == 0)
                        obj.Position(rect.Left + obj.GetWidth() / 2, rect.Position.y);
                    else
                    {
                        var last = _slides[_slides.Count - 1];
                        obj.Position(new Vector2
                        {
                            x = last.GetRect().Right + obj.GetWidth() / 2 + _offset,
                            y = rect.Position.y,
                        });
                    }
                    break;
                case UISliderHorizontalSettings.Direction.RIGHT_TO_LEFT:
                    if (_slides.Count == 0)
                        obj.Position(rect.Right - obj.GetWidth() / 2, rect.Position.y);
                    else
                    {
                        var last = _slides[_slides.Count - 1];
                        obj.Position(new Vector2
                        {
                            x = last.GetRect().Left - obj.GetWidth() / 2 - _offset,
                            y = rect.Position.y,
                        });
                    }
                    break;
            }
        }

        private void SetupChildrenVertical(UIObject obj, UISliderVerticalSettings settings)
        {
            var rect = GetRect();

            switch (settings.SliderDirection)
            {
                case UISliderVerticalSettings.Direction.TOP_TO_BOTTOM:
                    break;
                case UISliderVerticalSettings.Direction.BOTTOM_TO_TOP:
                    break;
            }
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var sliderSettings = settings as UISliderSettings;
            if (sliderSettings == null)
                throw new ArgumentException("UISlider: The given settings is not UIComponentSettings");

            if (sliderSettings.OrientationSettings == null)
            {
                sliderSettings.OrientationSettings = new UISliderHorizontalSettings
                {
                    SliderDirection = UISliderHorizontalSettings.Direction.LEFT_TO_RIGHT
                };
            }

            _sliderOrientation = sliderSettings.OrientationSettings;
            _height = sliderSettings.Height;
            _width = sliderSettings.Width;
            _offset = sliderSettings.Offset;

            _clickableHandler = UIClickable.Create(this, this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                var rect2d = area2d as RectangleArea2D;
                if (rect2d != null)
                {
                    rect2d.Height = GetHeight();
                    rect2d.Width = GetWidth();
                }
            };
            _clickableHandler.CanClick += () => IsActive;

            base.ApplySettings(settings);
        }
        
        public override float GetWidth()
        {
            return _width;
        }

        public override float GetHeight()
        {
            return _height;
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            _lastMousePos = worldPos;
        }

        public void MouseUp(Vector2 worldPos)
        {
            _isPressed = false;
            _lastMousePos = worldPos;
        }

        public void MouseDrag(Vector2 worldPos)
        {
            var diff = _lastMousePos - worldPos;

            mCore.Log("DIFF: {0}", diff);

            _lastMousePos = worldPos;
        }

        public void VerticalMove(float diff)
        {

        }

        public void HorizontalMove(float diff)
        {
            
        }
    }
}
