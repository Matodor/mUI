using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISliderSettings : UIComponentSettings
    {
        public float Height { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Offset { get; set; } = 0;
        public DirectionOfAddingSlides DirectionOfAddingSlides { get; set; } = DirectionOfAddingSlides.FORWARD;
        public SliderType SliderType { get; set; } = SliderType.HORIZONTAL;
    }

    public enum SliderType
    {
        HORIZONTAL = 0,
        VERTICAL = 1,
    }

    public enum DirectionOfAddingSlides
    {
        FORWARD = 0,
        BACKWARD = 1,
    }

    public class UISlider : UIComponent, IUIClickable
    {
        public UIClickable UIClickable { get { return _clickableHandler; } }

        private readonly List<UIObject> _slides;
        private UIClickable _clickableHandler;
        private SliderType _sliderType;
        private DirectionOfAddingSlides _directionOfAddingSlides;

        private float _height;
        private float _width;
        private float _offset;
        private float _lastMoveDiff;

        private bool _isPressed;
        private Vector2 _lastMousePos;

        private UISlider(UIObject parent) : base(parent)
        {
            _lastMoveDiff = 0;
            _isPressed = false;
            _slides = new List<UIObject>();
            OnAddedChildren += SetupChildren;
        }

        private void SetupChildren(UIObject obj)
        {
            if (_sliderType == SliderType.HORIZONTAL)
                SetupChildrenHorizontal(obj);
            else
                SetupChildrenVertical(obj);

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
            return Math.Abs(_lastMoveDiff) <= 0.001f;
        }

        private void SetupChildrenHorizontal(UIObject obj)
        {
            var rect = GetRect();

            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
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
                case DirectionOfAddingSlides.BACKWARD:
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

        private void SetupChildrenVertical(UIObject obj)
        {
            var rect = GetRect();

            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                    break;
                case DirectionOfAddingSlides.BACKWARD:
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

            _sliderType = sliderSettings.SliderType;
            _directionOfAddingSlides = sliderSettings.DirectionOfAddingSlides;
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
            if (!_isPressed)
                return;
            var diff = worldPos - _lastMousePos;
            Move(_sliderType == SliderType.HORIZONTAL ? diff.x : diff.y);
            _lastMousePos = worldPos;
        }
        
        private UIObject GetFirstSlide()
        {
            return _slides[0];
        }

        private UIObject GetLastSlide()
        {
            return _slides[_slides.Count - 1];
        }

        public void Move(float diff)
        {
            if (_sliderType == SliderType.HORIZONTAL)
            {
                HorizontalMove(diff);
                _lastMoveDiff = diff;
            }
            else
            {
                VerticalMove(diff);
                _lastMoveDiff = diff;
            }
        }

        internal override void Tick()
        {
            if (!_isPressed && Math.Abs(_lastMoveDiff) > 0.001f)
                Move(_lastMoveDiff * 0.99f * Time.deltaTime * 50);
            base.Tick();
        }

        private void VerticalMove(float diff)
        {
            if (_slides.Count == 0)
                return;

        }

        private void HorizontalMove(float diff)
        {
            if (_slides.Count == 0)
                return;
            var rect = GetRect();

            // move right
            if (diff > 0)
            {
                var firstRect = _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                    ? GetFirstSlide().GetRect()
                    : GetLastSlide().GetRect();

                var freeSpace = rect.Left - firstRect.Left;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }
            // move left
            else if (diff < 0)
            {
                var lastRect = _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                    ? GetLastSlide().GetRect()
                    : GetFirstSlide().GetRect();

                var freeSpace = lastRect.Right - rect.Right;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }

            for (int i = 0; i < _slides.Count; i++)
                _slides[i].Translate(diff, 0);
        }
    }
}
