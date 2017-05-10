using System;
using System.Collections.Generic;
using mUIApp.Input;
using mUIApp.Other;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Components
{
    public enum UIObjectOrientation
    {
        HORIZONTAL = 0,
        VERTICAL = 1,
    }

    public static partial class UIComponentsHelper
    {
        public static UISlider CreateSlider(this UIObject obj, UIObjectOrientation type = UIObjectOrientation.VERTICAL, string objName = "Slider")
        {
            var sliderView = obj.CreateView<UISliderView>();
            return new UISlider(sliderView, type).SetName("SliderController");
        }
    }

    public class UISliderView : UIView
    {
        public UISliderView(UIObject parent, params object[] param) : base(parent)
        {
        }
    }

    public class UISlider : UIClickableObj
    { 
        public override float PureWidth { get { return Parent.Width; } }
        public override float PureHeight { get { return Parent.Height; } }

        private readonly UIObjectOrientation _objectOrientation;
        private readonly List<UIView> _childs;
        private Vector2 _startDragPos;
        private Vector2 _lastDragPos;
        private Vector2 _lastDragDIff;
        private Vector2 _lastDragPath;
        private bool _sliderIsPressed;
        private bool _canPressButtons;
        private float _pressClickTime;

        public UISlider(UIView obj, UIObjectOrientation type) : base(obj)
        {
            _objectOrientation = type;
            _childs = new List<UIView>();
            _sliderIsPressed = false;
            _canPressButtons = true;

            IgnoreSortingOrder = true;
            OnTick += SliderTick;
            OnUIMouseUpEvent += OnMouseUpEvent;
            OnUIMouseDragEvent += OnMouseDragEvent;
            OnUIMouseDownEvent += OnMouseDownEvent;
            obj.CreateRectCamera();
        }

        private void SliderTick(UIObject sender)
        {
            if (!Active)
                return;

            if (!_sliderIsPressed)
            {
                if (Math.Abs(_lastDragDIff.x) > 0.001 || Math.Abs(_lastDragDIff.y) > 0.001)
                {
                    Move(_lastDragDIff);
                    _lastDragDIff *= 0.95f;
                }
            }
        }

        public bool CanPressButtons()
        {
            return _canPressButtons;
        } 

        private void OnMouseUpEvent(UIObject sender, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = currentPos;
            _sliderIsPressed = false;
        }

        private void OnMouseDragEvent(UIObject sender, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            if (!_sliderIsPressed)
                return;

            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            var diffPos = currentPos - _lastDragPos;

            _lastDragDIff = diffPos;
            _lastDragPos = currentPos;
            _lastDragPath += Move(diffPos);

            if (_canPressButtons && (Math.Abs(_lastDragPath.x) > 0.05 || Math.Abs(_lastDragPath.y) > 0.05))
            {
                _canPressButtons = false;
            }
        }

        private void OnMouseDownEvent(UIObject sender, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            if (mUI.Debug)
                DrawDebug();

            _canPressButtons = true;
            _lastDragPath = Vector2.zero;
            _sliderIsPressed = true;
            _startDragPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = _startDragPos;
            _lastDragDIff = Vector2.zero;
            _pressClickTime = Time.time;
        }

        private Vector2 Move(Vector2 diffPos)
        {
            if (_childs.Count != 0)
            {
                if (_objectOrientation == UIObjectOrientation.VERTICAL)
                {
                    if (diffPos.y <= 0)
                    {
                        var firstTop = _childs[0].Top;
                        if (Math.Abs(Parent.Top - firstTop) < Math.Abs(diffPos.y))
                            diffPos.y = Parent.Top - firstTop;
                        else if (Parent.Top >= firstTop)
                            diffPos.y = 0;
                    }
                    else
                    {
                        var lastBottom = _childs[_childs.Count - 1].Bottom;
                        if (Math.Abs(lastBottom - Parent.Bottom) < Math.Abs(diffPos.y))
                            diffPos.y = Parent.Bottom - lastBottom;
                        else if (Parent.Bottom <= lastBottom)
                            diffPos.y = 0;
                    }
                }
                else
                {

                }

                for (int i = 0; i < _childs.Count; i++)
                {
                    if (_objectOrientation == UIObjectOrientation.VERTICAL)
                        _childs[i].Translate(0, diffPos.y);
                    else
                        _childs[i].Translate(diffPos.x, 0);
                }
            }
            return diffPos;
        }


        private void DrawDebug()
        {
            for (int i = 0; i < _childs.Count; i++)
            {
                Debug.DrawLine(new Vector3(_childs[i].Left, _childs[i].Top),
                new Vector3(_childs[i].Right, _childs[i].Top));
                Debug.DrawLine(new Vector3(_childs[i].Right, _childs[i].Top),
                    new Vector3(_childs[i].Right, _childs[i].Bottom));
                Debug.DrawLine(new Vector3(_childs[i].Right, _childs[i].Bottom),
                    new Vector3(_childs[i].Left, _childs[i].Bottom));
                Debug.DrawLine(new Vector3(_childs[i].Left, _childs[i].Bottom),
                    new Vector3(_childs[i].Left, _childs[i].Top));
            }
        }

        private void SliderInject(UIObject uiObject)
        {
            foreach (var child in uiObject.Childs)
            {
                var obj = child as UIClickableObj;
                if (obj != null)
                {
                    var tmp = obj.CanClick;
                    obj.CanClick = (v) => tmp(v) && Active && InArea(v) && CanPressButtons();
                }
                SliderInject(child);
            } 
        }

        public T CreateChild<T>(float height, float width, float spacing = 0, params object[] param) where T : UIView
        {
            var newView = (T)Activator.CreateInstance(typeof(T), Parent, param);

            newView.SetHeight(height);
            newView.SetWidth(width);
            SliderInject(newView);

            if (_childs.Count == 0)
            {
                if (_objectOrientation == UIObjectOrientation.VERTICAL)
                    newView.Position(Parent.GetPos().x, Parent.Top - height/2);
            }
            else
            {
                if (_objectOrientation == UIObjectOrientation.VERTICAL)
                    newView.Position(Parent.GetPos().x, _childs[_childs.Count - 1].Bottom - height/2 - spacing);
            }

            _childs.Add(newView);
            return newView;
        }

        public override bool InArea(Vector2 screenPos)
        {
            return AreaChecker.InArea(Parent.Transform, mUI.UICamera.ScreenToWorldPoint(screenPos),
                new Bounds(new Vector3(0, 0), new Vector3(Parent.Width, Parent.Height)));
        }
    }
}
