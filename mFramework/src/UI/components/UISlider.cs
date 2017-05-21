using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISliderSettings : UIComponentSettings
    {
        public float Height { get; set; }
        public float Width { get; set; }
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

        private UISlider(UIObject parent) : base(parent)
        {
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
        }

        private void SetupChildrenHorizontal(UIObject obj, UISliderHorizontalSettings settings)
        {
            switch (settings.SliderDirection)
            {
                case UISliderHorizontalSettings.Direction.LEFT_TO_RIGHT:
                    break;
                case UISliderHorizontalSettings.Direction.RIGHT_TO_LEFT:
                    break;
            }
        }

        private void SetupChildrenVertical(UIObject obj, UISliderVerticalSettings settings)
        {
            
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
        }

        public void MouseUp(Vector2 worldPos)
        {
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }
    }
}
