using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
using mUIApp.Other;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public enum UISliderType
    {
        HORIZONTAL = 0,
        VERTICAL = 1,
    }

    public static class UISliderHelper
    {
        public static UISlider CreateSlider(this UIObject obj, string objName = "Slider", UISliderType type = UISliderType.VERTICAL)
        {
            var sliderView = obj.CreateView<UISliderView>(obj);
            return new UISlider(sliderView, type).SetName("SliderController");
        }
    }

    public class UISliderView : UIView
    {
        public UISliderView(UIObject parent) : base(parent)
        {
        }
    }

    public class UISlider : UIClickableObj
    { 
        public override float PureWidth { get { return Parent.Width; } }
        public override float PureHeight { get { return Parent.Height; } }

        private readonly UISliderType _sliderType;
        private readonly List<UIView> _childs;
        private Vector2 _startDragPos;
        private Vector2 _lastDragPos;
        private Vector2 _lastDragDIff;
        private bool _sliderIsPressed;
        private float _pressClickTime;

        public UISlider(UIView obj, UISliderType type) : base(obj)
        {
            _sliderType = type;
            _childs = new List<UIView>();
            _sliderIsPressed = false;

            OnTick += SliderTick;
            OnUIMouseUpEvent += OnMouseUpEvent;
            OnUIMouseDragEvent += OnMouseDragEvent;
            OnUIMouseDownEvent += OnMouseDownEvent;
            obj.CreateRectCamera();

            this.SetBoxArea();
        }

        private void SliderTick(UIObject sender)
        {
            if (!Active)
                return;

            if (!_sliderIsPressed)
            {
                Move(_lastDragDIff);
                _lastDragDIff *= 0.95f;
            }
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
             
            Move(diffPos);

            _lastDragDIff = diffPos;
            _lastDragPos = currentPos;

            //mUI.Log("diffPos: {0}", diffPos);
        }

        private void OnMouseDownEvent(UIObject sender, mUIMouseEvent mouseEvent)
        {
            if (!Active)
                return;

            if (mUI.Debug)
                DrawDebug();

            _sliderIsPressed = true;
            _startDragPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = _startDragPos;
            _lastDragDIff = Vector2.zero;
            _pressClickTime = Time.time;
        }

        private void Move(Vector2 diffPos)
        {
            if (_childs.Count != 0)
            {
                if (_sliderType == UISliderType.VERTICAL)
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
                    if (_sliderType == UISliderType.VERTICAL)
                        _childs[i].Translate(0, diffPos.y);
                    else
                        _childs[i].Translate(diffPos.x, 0);
                }
            }
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

        public T CreateChild<T>(float height, float width, float spacing = 0, params object[] param) where T : UIView
        {
            var newView = (T)Activator.CreateInstance(typeof(T), Parent, param);
            newView.SetHeight(height);
            newView.SetWidth(width);

            if (_childs.Count == 0)
                newView.Position(Parent.GetPos().x, Parent.Top - height/2);
            else
                newView.Position(Parent.GetPos().x, _childs[_childs.Count - 1].Bottom - height/2 - spacing);

            _childs.Add(newView);
            return newView;
        }

        protected override bool InArea(Vector2 screenPos)
        {
            return AreaChecker.InArea(Parent.Transform, mUI.UICamera.ScreenToWorldPoint(screenPos),
                new Bounds(new Vector3(0, 0), new Vector3(Parent.Width, Parent.Height)));
        }
    }
}
