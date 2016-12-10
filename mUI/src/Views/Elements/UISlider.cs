using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
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
        public static UISlider CreateSlider(this BaseView view, string objName = "Slider", UISliderType type = UISliderType.VERTICAL)
        {
            var sliderView = view.CreatePartial<UISliderView>("SliderView");
            return new UISlider(sliderView, type).SetName(objName);
        }
    }

    public class UISliderView : PartialView
    {
        public override void Create(object data)
        {
            
        }
    }

    public class UISlider : UIClickableObj
    { 
        public override float Width { get { return ParentView.Width; } }
        public override float Height { get { return ParentView.Height; } }
        public float PureWidth { get { return ParentView.PureWidth; } }
        public float PureHeight { get { return ParentView.PureHeight; } }

        private UISliderType _sliderType;
        private readonly List<PartialView> _childs;
        private Vector2 _startDragPos;
        private Vector2 _lastDragPos;
        private Vector2 _lastDragDIff;

        public UISlider(BaseView view, UISliderType type) : base(view)
        {
            _sliderType = type;
            _childs = new List<PartialView>();

            OnUIMouseUpEvent += OnMouseUpEvent;
            OnUIMouseDragEvent += OnMouseDragEvent;
            OnUIMouseDownEvent += OnMouseDownEvent;

            this.SetBoxArea();
        }

        private void OnMouseUpEvent(mUIMouseEvent mouseEvent)
        {
            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = currentPos;
        }

        private void OnMouseDragEvent(mUIMouseEvent mouseEvent)
        {
            var currentPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            var diffPos = currentPos - _lastDragPos;

            Move(diffPos);

            _lastDragDIff = diffPos;
            _lastDragPos = currentPos;

            mUI.Log("diffPos: {0}", diffPos);
        }

        private void Move(Vector2 diffPos)
        {
            if (_childs.Count != 0)
            {
                if (_sliderType == UISliderType.VERTICAL)
                {
                    if (diffPos.y <= 0)
                    {
                        var firstTop = _childs[0].TopAnchor;
                        if (Math.Abs(ParentView.TopAnchor - firstTop) < Math.Abs(diffPos.y))
                            diffPos.y = ParentView.TopAnchor - firstTop;
                        else if (ParentView.TopAnchor >= firstTop)
                            diffPos.y = 0;
                    }
                    else
                    {
                        var lastBottom = _childs[_childs.Count - 1].BottomAnchor;
                        if (Math.Abs(lastBottom - ParentView.BottomAnchor) < Math.Abs(diffPos.y))
                            diffPos.y = ParentView.BottomAnchor - lastBottom;
                        else if (ParentView.BottomAnchor <= lastBottom)
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

        private void OnMouseDownEvent(mUIMouseEvent mouseEvent)
        {
            if (mUI.Debug)
                DrawDebug();
            _startDragPos = mUI.UICamera.ScreenToWorldPoint(mouseEvent.MouseScreenPos);
            _lastDragPos = _startDragPos;
        }

        private void DrawDebug()
        {
            Debug.DrawLine(new Vector3(ParentView.LeftAnchor, ParentView.TopAnchor),
                new Vector3(ParentView.RightAnchor, ParentView.TopAnchor));
            Debug.DrawLine(new Vector3(ParentView.RightAnchor, ParentView.TopAnchor),
                new Vector3(ParentView.RightAnchor, ParentView.BottomAnchor));
            Debug.DrawLine(new Vector3(ParentView.RightAnchor, ParentView.BottomAnchor),
                new Vector3(ParentView.LeftAnchor, ParentView.BottomAnchor));
            Debug.DrawLine(new Vector3(ParentView.LeftAnchor, ParentView.BottomAnchor),
                new Vector3(ParentView.LeftAnchor, ParentView.TopAnchor));

            for (int i = 0; i < _childs.Count; i++)
            {
                Debug.DrawLine(new Vector3(_childs[i].LeftAnchor, _childs[i].TopAnchor),
                new Vector3(_childs[i].RightAnchor, _childs[i].TopAnchor));
                Debug.DrawLine(new Vector3(_childs[i].RightAnchor, _childs[i].TopAnchor),
                    new Vector3(_childs[i].RightAnchor, _childs[i].BottomAnchor));
                Debug.DrawLine(new Vector3(_childs[i].RightAnchor, _childs[i].BottomAnchor),
                    new Vector3(_childs[i].LeftAnchor, _childs[i].BottomAnchor));
                Debug.DrawLine(new Vector3(_childs[i].LeftAnchor, _childs[i].BottomAnchor),
                    new Vector3(_childs[i].LeftAnchor, _childs[i].TopAnchor));
            }
        }
        
        public T CreateChild<T>(float height, float width, float spacing = 0, object data = null) where T : PartialView, new()
        {
            var child = ParentView.CreatePartial<T>("element", data);
            child.SetHeight(height); 
            child.SetWidth(width);

            if (_childs.Count == 0)
                child.Position(ParentView.GetPos().x, ParentView.TopAnchor - height/2);
            else
                child.Position(ParentView.GetPos().x, _childs[_childs.Count - 1].BottomAnchor - height/2 - spacing);

            _childs.Add(child);
            return (T)child;
        }

        protected override bool InArea(Vector2 screenPos)
        {
            return AreaChecker.InArea(ParentView.Transform, mUI.UICamera.ScreenToWorldPoint(screenPos),
                new Bounds(new Vector3(0, 0), new Vector3(ParentView.Width, ParentView.Height)));
        }
    }
}
