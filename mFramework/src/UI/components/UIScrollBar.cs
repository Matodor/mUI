using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIScrollBarSettings : UIComponentSettings
    {
        public UIButtonSettings ButtonSettings { get; set; }
        public UnityEngine.Sprite BarSprite { get; set; }  
        public UIObjectOrientation Orientation { get; set; }
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 1;
        public float Default { get; set; } = 0.5f;
        public float Step { get; set; } = 0.2f;
    }

    public class UIScrollBar : UIComponent
    {
        public event Action<UIScrollBar> OnChanged;

        public float Value { get { return _value; } }
        public float Value01 { get { return _value01; } }
        public float Step { get { return _step; } }

        private UIObjectOrientation _orientation;
        private UIButton _barButton;
        private UISprite _bar;

        //private float _pointPos;
        private float _movedPos;
        private float _value;
        private float _value01;
        private float _minValue, _maxValue;
        private float _stepClamp;
        private float _step;

        private bool _isPressed;
        private Vector2 _lastDragPos;
        private MouseEventListener _mouseEventListener;

        private const float DIVIDE = 10000f;


        protected UIScrollBar(UIObject parent) : base(parent)
        {
            _isPressed = false;
            _mouseEventListener = MouseEventListener.Create();
            _mouseEventListener.OnMouseDrag += OnMouseDrag;
        }
        
        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var scrollBarSettings = settings as UIScrollBarSettings;
            if (scrollBarSettings == null)
                throw new ArgumentException("UIScrollBar: The given settings is not UIScrollBarSettings");
            if (scrollBarSettings.ButtonSettings == null)
                throw new Exception("UIScrollBar: The given ButtonSettings is null");

            _orientation = scrollBarSettings.Orientation;
            _minValue = Mathf.Min(scrollBarSettings.Min, scrollBarSettings.Max);
            _maxValue = Mathf.Max(scrollBarSettings.Min, scrollBarSettings.Max);
            _value = mMath.Clamp(scrollBarSettings.Default, _minValue, _maxValue);
            _value01 = CalcValue01(_value, _minValue, _maxValue);

            _bar = Component<UISprite>(new UISpriteSettings
            {
                Sprite = scrollBarSettings.BarSprite
            });

            _barButton = Component<UIButton>(scrollBarSettings.ButtonSettings);
            _barButton.OnMouseDown += OnMouseDown;
            _barButton.OnMouseUp += OnMouseUp;


            _bar.SortingOrder(0);
            _barButton.SortingOrder(1);

            if (_orientation == UIObjectOrientation.VERTICAL)
                _bar.Rotate(90);

            SetStep(scrollBarSettings.Step);
            SetValue(_value);

            _movedPos = GetPointPos(_value01);
            base.ApplySettings(settings);
        }

        public UIScrollBar SetStep(float step)
        {
            _step = mMath.Clamp(step, 0.0001f, _maxValue - _minValue);
            var v = _step / ((_maxValue - _minValue) / DIVIDE);
            _stepClamp = (1f / DIVIDE) * v;
            SetValue(_value);
            return this;
        }

        public static float CalcValue01(float val, float min, float max)
        {
            return (val - min) / (max - min);
        }

        public UIScrollBar SetValue(float value)
        {
            value = mMath.Clamp(value, _minValue, _maxValue);
            var value01 = CalcValue01(value, _minValue, _maxValue);
            SetValue01(value01);
            return this;
        }

        public void SetValue01(float value01)
        {
            value01 = mMath.Clamp(value01, 0f, 1f);

            var v = value01 / _stepClamp;
            var c = (float)Math.Truncate(v);
            var o = v - c;

            if (o < 0.5f)
                _value01 = _stepClamp * c;
            else
                _value01 = _stepClamp * c + _stepClamp;
            _value01 = mMath.Clamp(_value01, 0f, 1f);

            float prevValue = _value;
            _value = _minValue + (_maxValue - _minValue) * _value01;

            if (prevValue != _value)
            {
                if (_orientation == UIObjectOrientation.HORIZONTAL)
                    _barButton.Position(GetPointPos(_value01), _barButton.Position().y);
                else if (_orientation == UIObjectOrientation.VERTICAL)
                    _barButton.Position(_barButton.Position().x, GetPointPos(_value01));
                OnChanged?.Invoke(this);
            }
        }

        private void OnMouseDrag(MouseEvent mouseEvent)
        {
            if (!_isPressed)
                return;

            var worldPos = UIClickable.WorldPos(mouseEvent);
            var diff = worldPos - _lastDragPos;
            _lastDragPos = worldPos;
            Move(_orientation == UIObjectOrientation.VERTICAL ? diff.y : diff.x);
        }

        private void Move(float diff)
        {
            var barRect = _bar.GetRect();
            if (_orientation == UIObjectOrientation.HORIZONTAL)
            {
                if (diff > 0)
                {
                    var freeSpace = barRect.Right - _movedPos;
                    if (Math.Abs(diff) > freeSpace)
                        diff = Math.Sign(diff) * freeSpace;
                }
                else if (diff < 0)
                {
                    var freeSpace = _movedPos - barRect.Left;
                    if (Math.Abs(diff) > freeSpace)
                        diff = Math.Sign(diff) * freeSpace;
                }
                _movedPos += diff;
                SetValue01((barRect.Left - _movedPos) / (barRect.Left - barRect.Right));
            }
            else if (_orientation == UIObjectOrientation.VERTICAL)
            {
                var top = mMath.GetRotatedPoint(barRect.Position, new Vector2(barRect.Right, barRect.Position.y), _bar.Rotation());
                var bottom = mMath.GetRotatedPoint(barRect.Position, new Vector2(barRect.Left, barRect.Position.y), _bar.Rotation());

                if (diff > 0)
                {
                    var freeSpace = top.y - _movedPos;
                    if (Math.Abs(diff) > freeSpace)
                        diff = Math.Sign(diff) * freeSpace;
                }
                else if (diff < 0)
                {
                    var freeSpace = _movedPos - bottom.y;
                    if (Math.Abs(diff) > freeSpace)
                        diff = Math.Sign(diff) * freeSpace;
                }
                _movedPos += diff;
                SetValue01((top.y - _movedPos) / (top.y - bottom.y));
            }
        }

        private float GetPointPos(float value01)
        {
            var barRect = _bar.GetRect();
            if (_orientation == UIObjectOrientation.HORIZONTAL)
            {
                return barRect.Left + (barRect.Right - barRect.Left) * value01;
            }
            else if (_orientation == UIObjectOrientation.VERTICAL)
            {
                var top = mMath.GetRotatedPoint(barRect.Position, new Vector2(barRect.Right, barRect.Position.y), _bar.Rotation());
                var bottom = mMath.GetRotatedPoint(barRect.Position, new Vector2(barRect.Left, barRect.Position.y), _bar.Rotation());
                return top.y - (top.y - bottom.y) * value01;
            }
            return 0;
        }

        private bool OnMouseUp(UIButton button, Vector2 worldPos)
        {
            _lastDragPos = worldPos;
            _isPressed = false;
            return true;
        }

        private bool OnMouseDown(UIButton button, Vector2 worldPos)
        {
            _lastDragPos = worldPos;
            _isPressed = true;
            _movedPos = GetPointPos(_value01);
            return true;
        }

        public override float GetHeight()
        {
            return _barButton.GetHeight() > _bar.GetHeight() 
                ? _barButton.GetHeight() 
                : _bar.GetHeight();
        }

        public override float GetWidth()
        {
            return _bar.GetWidth();
        }
    }
}
