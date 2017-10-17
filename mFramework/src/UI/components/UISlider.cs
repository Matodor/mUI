using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISliderSettings : UIComponentSettings
    {
        public float Height = 0;
        public float Width = 0;
        public float Offset = 0;
        public DirectionOfAddingSlides DirectionOfAddingSlides = DirectionOfAddingSlides.FORWARD;
        public UIObjectOrientation SliderType = UIObjectOrientation.HORIZONTAL;
    }

    public enum DirectionOfAddingSlides
    {
        FORWARD = 0,
        BACKWARD = 1,
    }

    public class UISlider : UIComponent, IUIClickable, IView
    {
        public UIObjectOrientation Orientation => _sliderOrientation;
        public UIClickable UIClickable => _clickableHandler;

        public const float SLIDER_MAX_PATH_TO_CLICK = 0.03f;
        public const float SLIDER_MIN_DIFF_TO_MOVE = 0.0001f;

        protected DirectionOfAddingSlides _directionOfAddingSlides;
        protected bool _isPressed;

        private UIClickable _clickableHandler;
        private UICamera _camera;
        private UIObjectOrientation _sliderOrientation;

        private float _height;
        private float _width;
        private float _offset;
        private float _lastMoveDiff;
        private float _dragPath;

        private Vector2 _lastMousePos;

        private List<Pair<IUIClickable, MouseEvent>> _clickNext;

        protected override void Init()
        {
            _lastMoveDiff = 0;
            _isPressed = false;
            _clickNext = new List<Pair<IUIClickable, MouseEvent>>();
            
            СhildObjectAdded += OnСhildObjectAdded;
        }

        protected virtual void OnСhildObjectAdded(UIObject sender, AddedСhildObjectEventArgs e)
        {
            if (_sliderOrientation == UIObjectOrientation.HORIZONTAL)
                SetupChildrenHorizontal(e.AddedObject);
            else
                SetupChildrenVertical(e.AddedObject);

            SetupChilds(sender, e);
        }

        private void SetupChilds(UIObject sender, AddedСhildObjectEventArgs e)
        {
            if (e.AddedObject is IUIClickable uiClickable)
            {
                uiClickable.UIClickable.CanMouseDown += CanChildsMouseDown;
                uiClickable.UIClickable.CanMouseUp += CanChildsMouseUp;
            }

            e.AddedObject.СhildObjectAdded += SetupChilds;
        }
        
        private void UpdateViewport()
        {
            var viewportPos = mUI.UICamera.Camera.WorldToViewportPoint(Position());
            var lb = new Vector2(mUI.UICamera.Left, mUI.UICamera.Bottom);

            var sliderScreenWidthScale = GetWidth() / mUI.UICamera.Width;
            var sliderScreenHeightScale = GetHeight() / mUI.UICamera.Height;

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

        protected virtual void SetupChildrenHorizontal(UIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (ChildsCount <= 1)
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
                } break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (ChildsCount <= 1)
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
                } break;
            }
        }

        protected virtual void SetupChildrenVertical(UIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (ChildsCount <= 1)
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
                }
                break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (ChildsCount <= 1)
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
                }
                break;
            }
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISliderSettings sliderSettings))
                throw new ArgumentException("UISlider: The given settings is not UIComponentSettings");

            _sliderOrientation = sliderSettings.SliderType;
            _directionOfAddingSlides = sliderSettings.DirectionOfAddingSlides;
            _height = sliderSettings.Height;
            _width = sliderSettings.Width;
            _offset = sliderSettings.Offset;

            _clickableHandler = UIClickable.Create(this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                if (!(area2d is RectangleArea2D rect2d))
                    return;

                rect2d.Height = GetHeight();
                rect2d.Width = GetWidth();
            };

            //_clickableHandler.CanMouseDown += (h, e) => IsActive;
            //_clickableHandler.CanMouseDrag += (h, e) => IsActive;
            //_clickableHandler.CanMouseUp += (h, e) => IsActive;

            _camera = UICamera.Create(new UICameraSettings());
            _camera.GameObject.SetParentTransform(gameObject);
            _camera.SetOrthographicSize(GetHeight() / 2);
            
            gameObject.transform.position = new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y,
                gameObject.transform.position.z + SortingOrder()
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

        public virtual void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            _lastMousePos = worldPos;
        }

        public virtual void MouseUp(Vector2 worldPos)
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

        public virtual void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            var diff = worldPos - _lastMousePos;
            if (_sliderOrientation == UIObjectOrientation.HORIZONTAL)
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
        }

        protected UIObject GetFirstSliderItem()
        {
            return this[0];
        }

        protected UIObject GetLastSliderItem()
        {
            return this[ChildsCount - 1];
        }

        public void Move(float diff)
        {
            if (_sliderOrientation == UIObjectOrientation.HORIZONTAL)
                HorizontalMove(diff);
            else
                VerticalMove(diff);
        }

        internal override void Tick()
        {
            if (!IsActive)
                return;

            if (!_isPressed && Math.Abs(_lastMoveDiff) > SLIDER_MIN_DIFF_TO_MOVE)
                Move(_lastMoveDiff * 0.99f * Time.deltaTime * 50);
            base.Tick();
        }

        protected virtual void VerticalMove(float diff)
        {
            if (ChildsCount == 0)
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

            for (int i = 0; i < ChildsCount; i++)
                this[i].Translate(0, diff);

            _lastMoveDiff = diff;
        }

        private bool InSliderArea(UIRect rect)
        {
            var sliderRect = GetRect();

            if (_sliderOrientation == UIObjectOrientation.HORIZONTAL)
            {
                if (rect.Left > sliderRect.Right)
                    return false;
                if (rect.Right < sliderRect.Left)
                    return false;
            }
            else 
            {
                if (rect.Bottom > sliderRect.Top)
                    return false;
                if (rect.Top < sliderRect.Bottom)
                    return false;
            }

            return true;
        }

        protected virtual void HorizontalMove(float diff)
        {
            if (ChildsCount == 0)
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
            
            for (int i = 0; i < ChildsCount; i++)
                this[i].Translate(diff, 0);

            _lastMoveDiff = diff;
        }
    }
}
