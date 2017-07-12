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
        public UIObjectOrientation SliderType { get; set; } = UIObjectOrientation.HORIZONTAL;
    }

    public enum DirectionOfAddingSlides
    {
        FORWARD = 0,
        BACKWARD = 1,
    }

    public class UISlider : UIComponent, IUIClickable
    {
        public UIClickable UIClickable { get { return _clickableHandler; } }

        private const float SLIDER_MAX_PATH_TO_CLICK = 0.03f;
        private const float SLIDER_MIN_DIFF_TO_MOVE = 0.0001f;

        private readonly List<UIObject> _slides;
        private UIClickable _clickableHandler;
        private UICamera _camera;
        private UIObjectOrientation _sliderType;
        private DirectionOfAddingSlides _directionOfAddingSlides;

        private float _height;
        private float _width;
        private float _offset;
        private float _lastMoveDiff;
        private float _dragPath;

        private bool _isPressed;
        private Vector2 _lastMousePos;

        private static int _countSliders = 0;
        private readonly List<Pair<IUIClickable, MouseEvent>> _clickNext;

        private UISlider(UIObject parent) : base(parent)
        {
            _countSliders++;
            _lastMoveDiff = 0;
            _isPressed = false;
            _slides = new List<UIObject>();
            _clickNext = new List<Pair<IUIClickable, MouseEvent>>();
            
            AddedСhildObject += OnAddedСhildObject;
        }

        private void OnAddedСhildObject(UIObject sender, AddedСhildObjectEventArgs e)
        {
            if (_sliderType == UIObjectOrientation.HORIZONTAL)
                SetupChildrenHorizontal(e.AddedObject);
            else
                SetupChildrenVertical(e.AddedObject);

            _slides.Add(e.AddedObject);
            SetupChilds(sender, e);
        }

        private void SetupChilds(UIObject sender, AddedСhildObjectEventArgs e)
        {
            var uiClickable = e.AddedObject as IUIClickable;
            if (uiClickable != null)
            {
                uiClickable.UIClickable.CanMouseDown += CanChildsMouseDown;
                uiClickable.UIClickable.CanMouseUp += CanChildsMouseUp;
            }

            e.AddedObject.AddedСhildObject += SetupChilds;
        }

        ~UISlider()
        {
            _countSliders--;
        }

        public T ChildView<T>(params object[] @params) where T : UIView
        {
            return ChildView<T>(new UIViewSettings
            {
                Width = GetWidth(),
                Height = GetHeight()
            }, @params);
        }

        public T ChildView<T>(UIViewSettings settings, params object[] @params) where T : UIView
        {
            return UIView.Create<T>(settings, this, @params);
        }

        private void UpdateViewport()
        {
            var viewportPos = mUI.UICamera.Camera.WorldToViewportPoint(Position());
            var lb = new Vector2(mUI.UICamera.Left, mUI.UICamera.Bottom);

            var sliderScreenWidthScale = GetWidth() / (mUI.UICamera.Right - mUI.UICamera.Left);
            var sliderScreenHeightScale = GetHeight() / (mUI.UICamera.Top - mUI.UICamera.Bottom);

            var minViewport = mUI.UICamera.Camera.WorldToViewportPoint(
                new Vector3(lb.x + GetWidth() / 2, lb.y + GetHeight() / 2, 0));

            _camera.Camera.rect = new Rect(
                viewportPos.x - minViewport.x,
                viewportPos.y - minViewport.y,
                sliderScreenWidthScale,
                sliderScreenHeightScale
            );
        }
      
        private static bool CanChildsMouseUp(IUIClickable handler, MouseEvent @event)
        {
            return false;
        }

        private bool CanChildsMouseDown(IUIClickable handler, MouseEvent @event)
        {
            if (_clickableHandler.InArea(UIClickable.WorldPos(@event)) &&
                _dragPath < SLIDER_MAX_PATH_TO_CLICK)
            {
                _clickNext.Add(new Pair<IUIClickable, MouseEvent>(handler, @event));
                return true;
            }
            return false;   
        }

        private void SetupChildrenHorizontal(UIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                    if (_slides.Count == 0)
                        obj.Position(rect.Left + obj.GetWidth() / 2, rect.Position.y);
                    else
                    {
                        var last = GetLastSliderItem();
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
                        var last = GetLastSliderItem();
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
                    if (_slides.Count == 0)
                        obj.Position(rect.Position.x, rect.Top - obj.GetHeight() / 2);
                    else
                    {
                        var last = GetLastSliderItem();
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Bottom - obj.GetHeight() / 2 - _offset,
                        });
                    }
                    break;
                case DirectionOfAddingSlides.BACKWARD:
                    if (_slides.Count == 0)
                        obj.Position(rect.Position.x, rect.Bottom + obj.GetHeight() / 2);
                    else
                    {
                        var last = GetLastSliderItem();
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Top + obj.GetHeight() / 2 + _offset,
                        });
                    }
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

            _clickableHandler = UIClickable.Create(this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                var rect2d = area2d as RectangleArea2D;
                if (rect2d == null)
                    return;

                rect2d.Height = GetHeight();
                rect2d.Width = GetWidth();
            };

            _clickableHandler.CanMouseDown += (h, e) => IsActive;
            _clickableHandler.CanMouseDrag += (h, e) => IsActive;
            _clickableHandler.CanMouseUp += (h, e) => IsActive;

            _camera = UICamera.Create(new UICameraSettings());
            _camera.GameObject.SetParent(_gameObject);
            _camera.SetOrthographicSize(GetHeight() / 2);
            
            _gameObject.transform.position = new Vector3(
                _gameObject.transform.position.x,
                _gameObject.transform.position.y,
                _gameObject.transform.position.z + _countSliders
            );

            Translated += (s, e) => UpdateViewport();
            UpdateViewport();

            base.ApplySettings(settings);
        }
        
        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
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
            
            if (_clickNext.Count > 0)
            {
                if (_dragPath < SLIDER_MAX_PATH_TO_CLICK)
                    _clickNext.ForEach(e => e.First.MouseUp(worldPos));
                else 
                    _clickNext.ForEach(e => e.First.MouseUp(new Vector2(float.MinValue, float.MinValue))); // fake pos
                _clickNext.Clear();
            }

            _dragPath = 0;
        }

        public void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            var diff = worldPos - _lastMousePos;
            if (_sliderType == UIObjectOrientation.HORIZONTAL)
            {
                _dragPath += Math.Abs(diff.x);
                HorizontalMove(diff.x);
            }
            else
            {
                _dragPath += Math.Abs(diff.y);
                VerticalMove(diff.y);
            }

            _lastMousePos = worldPos;
            //mCore.Log("_movePath: {0}", _dragPath.ToString("f"));
        }

        private UIObject GetFirstSliderItem()
        {
            return _slides[0];
        }

        private UIObject GetLastSliderItem()
        {
            return _slides[_slides.Count - 1];
        }

        public void Move(float diff)
        {
            if (_sliderType == UIObjectOrientation.HORIZONTAL)
                HorizontalMove(diff);
            else
                VerticalMove(diff);
        }

        internal override void Tick()
        {
            if (!_isPressed && Math.Abs(_lastMoveDiff) > SLIDER_MIN_DIFF_TO_MOVE)
                Move(_lastMoveDiff * 0.99f * Time.deltaTime * 50);
            base.Tick();
        }

        private void VerticalMove(float diff)
        {
            if (_slides.Count == 0)
                return;

            UIRect topRect, bottomRect;
            if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
            {
                topRect = GetFirstSliderItem().GetRect();
                bottomRect = GetLastSliderItem().GetRect();
            }
            else
            {
                topRect = GetLastSliderItem().GetRect();
                bottomRect = GetFirstSliderItem().GetRect();
            }

            var rect = GetRect();
            var pureHeight = topRect.Top - bottomRect.Bottom;

            if (pureHeight <= GetHeight())
                return;

            // move top
            if (diff > 0)
            { 
                var freeSpace = rect.Bottom - bottomRect.Bottom;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }
            // move bottom
            else
            {
                var freeSpace = topRect.Top - rect.Top;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }

            for (int i = 0; i < _slides.Count; i++)
                _slides[i].Translate(0, diff);

            _lastMoveDiff = diff;
        }

        private void HorizontalMove(float diff)
        {
            if (_slides.Count == 0)
                return;

            UIRect leftRect, rightRect;
            if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
            {
                leftRect = GetFirstSliderItem().GetRect();
                rightRect = GetLastSliderItem().GetRect();
            }
            else
            {
                leftRect = GetLastSliderItem().GetRect();
                rightRect = GetFirstSliderItem().GetRect();
            }

            var rect = GetRect();
            var pureWidth = rightRect.Right - leftRect.Left;

            if (pureWidth <= GetWidth())
                return;

            // move right
            if (diff > 0)
            {
                var freeSpace = rect.Left - leftRect.Left;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }
            // move left
            else if (diff < 0)
            {
                var freeSpace = rightRect.Right - rect.Right;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }

            for (int i = 0; i < _slides.Count; i++)
                _slides[i].Translate(diff, 0);

            _lastMoveDiff = diff;
        }
    }
}
