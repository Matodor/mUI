using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
using mUIApp.Views;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.src.Views.Elements
{
    public static partial class UIElementsHelper
    {
        public static UIScrollBar CreateScrollBar(this UIObject obj, float min, float max, Sprite bar, Sprite point, 
            UIObjectOrientation orientation = UIObjectOrientation.VERTICAL,string objName = "Scroll bar")
        {
            return new UIScrollBar(obj, min, max, bar, point, orientation).SetName(objName);
        }
    }

    public class UIScrollBar : UIClickableObj
    {
        public override float PureWidth { get; }
        public override float PureHeight { get; }
        public float CurrentValue { get { return _currentValue; } }
        public float CurrentPos { get { return _pos; } }

        public UISprite Bar { get { return _bar; } }
        public UISprite Point { get { return _point; } }

        public event Action<UIScrollBar> OnChange;

        private readonly UISprite _bar, _point;
        private readonly UIObjectOrientation _orientation;

        private bool _isPressed;

        private float _currentValue;
        private float _min;
        private float _max;
        private float _pos, _stepClamp;
        private float _currentPointPos;

        private Vector2 _startDragPos;
        private Vector2 _lastDragPos;
        private Vector2 _lastDragDIff;
        private Vector2 _lastDragPath;

        private const float _divide = 10000f;

        public UIScrollBar(UIObject obj, float min, float max, Sprite barSprite, Sprite pointSprite,
            UIObjectOrientation orientation) : base(obj)
        {
            OnUIMouseDownEvent += MouseDownEvent;
            OnUIMouseDragEvent += MouseDragEvent; 
            OnUIMouseUpEvent += MouseUpEvent;

            _min = min;
            _max = max;

            _stepClamp = 1f/ _divide;

            _orientation = orientation;
            _bar = this.CreateSprite(barSprite);
            _point = _bar.CreateSprite(pointSprite);

            PureHeight = _point.CurrentSprite().bounds.size.y * 2;
            PureWidth = _bar.CurrentSprite().bounds.size.x;
            if (_orientation == UIObjectOrientation.VERTICAL)
            {
                _bar.Rotate(90);
                _currentPointPos = _point.Transform.position.y;
            }
            else
            {
                _currentPointPos = _point.Transform.position.x;
            }
        }

        public UIScrollBar Step(float step)
        {
            step = mUI.Сlamp(step, 0.0001f, _max - _min);
            float v = step / ((_max - _min) / _divide);
            _stepClamp = (1f / _divide) * v;
            Update();
            return this;
        }

        private void MouseUpEvent(UIObject uiObject, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = currentPos;
            _isPressed = false;
        }

        private void MouseDragEvent(UIObject uiObject, mUIMouseEvent mouseEvent)
        {
            if (!Active || !_isPressed)
                return;

            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            var diffPos = currentPos - _lastDragPos;

            _lastDragDIff = diffPos;
            _lastDragPos = currentPos;
            _lastDragPath += diffPos;

            Move(diffPos);
        }

        private void MouseDownEvent(UIObject uiObject, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            _isPressed = true;
            _lastDragPath = Vector2.zero;
            _startDragPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = _startDragPos;
            _lastDragDIff = Vector2.zero;
        }

        private void Move(Vector2 diffPos)
        {
            if (_orientation == UIObjectOrientation.VERTICAL)
            {
                
            }
            else
            {
                if (diffPos.x < 0)
                {
                    if (Math.Abs(Left - _point.Transform.position.x) < Math.Abs(diffPos.x))
                        diffPos.x = Left - _point.Transform.position.x;
                    else if (Left >= _point.Transform.position.x)
                        diffPos.x = 0;
                }
                else
                {
                    if (Math.Abs(Right - _point.Transform.position.x) < Math.Abs(diffPos.x))
                        diffPos.x = Right - _point.Transform.position.x;
                    else if (Right <= _point.Transform.position.x)
                        diffPos.x = 0;
                }
            }

            if (_orientation == UIObjectOrientation.VERTICAL)
            {
                _currentPointPos += diffPos.y;
                //_point.Translate(0, diffPos.y);
            }
            else
            {
                _currentPointPos += diffPos.x;
                //_point.Translate(diffPos.x, 0);
            }

            Update();
        }

        public UIScrollBar SetValue(float value)
        {
            value = mUI.Сlamp(value, _min, _max);
            float pos = (value - _min)/(_max - _min);
            if (_orientation == UIObjectOrientation.HORIZONTAL)
                _currentPointPos = Left + (Right - Left) * pos;
            else
                _currentPointPos = Top - (Top - Bottom) * pos;
            Update();
            return this;
        }

        private void Update()
        {
            if (_orientation == UIObjectOrientation.VERTICAL)
            {
                _pos = (Top - _currentPointPos) / (Top - Bottom);
            }
            else
            {
                _pos = (Left - _currentPointPos) / (Left - Right);
            }

            var v = _pos/_stepClamp;
            var c = (float)Math.Truncate(v);
            var o = v - c;

            if (o < 0.5f)
                _pos = _stepClamp*c; 
            else
                _pos = _stepClamp*c + _stepClamp;
            _pos = mUI.Сlamp(_pos, 0f, 1f);

            float prevValue = _currentValue;
            _currentValue = _min + (_max - _min) * _pos;

            var newPos = new Vector3(
                _point.Transform.position.x,
                _point.Transform.position.y,
                _point.Transform.position.z
            );

            if (_orientation == UIObjectOrientation.HORIZONTAL)
                newPos.x = Left + (Right - Left)*_pos;
            else
                newPos.y = Top - (Top - Bottom) * _pos;
            _point.Transform.position = newPos;

            if (prevValue != _currentValue)
                OnChange?.Invoke(this);
        }

        public override bool InArea(Vector2 screenPos)
        {
            return AreaChecker.InArea(Transform, mUI.UICamera.ScreenToWorldPoint(screenPos),
                new Bounds(new Vector3(0, 0), new Vector3(PureWidth, PureHeight)));
        }
    }
}
