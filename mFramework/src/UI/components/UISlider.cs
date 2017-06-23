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

        private const float SLIDER_MAX_PATH_TO_CLICK = 0.03f;
        private const float SLIDER_MIN_DIFF_TO_MOVE = 0.0001f;

        private readonly List<UIObject> _slides;
        private UIClickable _clickableHandler;
        private UICamera _camera;
        private SliderType _sliderType;
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
        //private readonly Material _sliderMaterial;

        private UISlider(UIObject parent) : base(parent)
        {
            _countSliders++;
            _lastMoveDiff = 0;
            _isPressed = false;
            _slides = new List<UIObject>();
            _clickNext = new List<Pair<IUIClickable, MouseEvent>>();
            //_sliderMaterial = new Material(Shader.Find("mFramework/Sprites/SpriteClipping"));
            
            OnAddedChildren += SetupChildren;
        }

        ~UISlider()
        {
            _countSliders--;
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
            {
                uiClickable.UIClickable.CanMouseDown += CanChildsMouseDown;
                uiClickable.UIClickable.CanMouseUp += CanChildsMouseUp;
            }

            //var uiRenderer = uiObject as IUIRenderer;
            //if (uiRenderer != null)
            //{
            //    uiRenderer.UIRenderer.material = _sliderMaterial;
            //}

            uiObject.ForEachChildren(OnRecursiveAddedChildren);
            uiObject.OnAddedChildren += OnRecursiveAddedChildren;
        }

        private bool CanChildsMouseUp(IUIClickable handler, MouseEvent @event)
        {
            return false;
        }

        private bool CanChildsMouseDown(IUIClickable handler, MouseEvent @event)
        {
            if (_clickableHandler.InArea(_clickableHandler.WorldPos(@event)) &&
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
                        var last = GetLastSlide();
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
                        var last = GetLastSlide();
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
                        var last = GetLastSlide();
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
                        var last = GetLastSlide();
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

            _clickableHandler = UIClickable.Create(this, this, AreaType.RECTANGLE);
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

            OnTranslate += (e) => UpdateViewport();
            UpdateViewport();

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
            if (_sliderType == SliderType.HORIZONTAL)
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
            var rect = GetRect();

            // move top
            if (diff > 0)
            { 
                var lastRect = _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                    ? GetLastSlide().GetRect()
                    : GetFirstSlide().GetRect();

                var freeSpace = rect.Bottom - lastRect.Bottom;
                if (Math.Abs(diff) > freeSpace)
                    diff = Math.Sign(diff) * freeSpace;
            }
            // move bottom
            else
            {
                var firstRect = _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                    ? GetFirstSlide().GetRect()
                    : GetLastSlide().GetRect();

                var freeSpace = firstRect.Top - rect.Top;
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

            _lastMoveDiff = diff;
        }
    }
}
