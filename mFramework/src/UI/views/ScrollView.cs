using System;
using System.Collections.Generic;
using System.Linq;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class ScrollViewProps : UIViewProps
    {
        public virtual FlexboxLayoutProps FlexboxProps { get; set; }
        public override ushort? StencilId { get; set; } = 1 << 2;
    }

    public class ScrollView : UIView, IUIDragable
    {
        public event UIMouseEvent MouseDown;
        public event UIMouseEvent MouseUp;
        public event UIMouseEvent MouseDrag;

        public event UIMouseAllowEvent CanMouseDown
        {
            add => _canMouseDownEvents.Add(value);
            remove => _canMouseDownEvents.Remove(value);
        }

        public event UIMouseAllowEvent CanMouseUp
        {
            add => _canMouseUpEvents.Add(value);
            remove => _canMouseUpEvents.Remove(value);
        }

        public event UIMouseAllowEvent CanMouseDrag
        {
            add => _canMouseDragEvents.Add(value);
            remove => _canMouseDragEvents.Remove(value);
        }
        
        public bool IgnoreByHandler { get; set; }
        public bool IsPressed { get; protected set; }

        public IAreaChecker AreaChecker { get; set; }

        private const float MAX_PATH_TO_CLICK = 0.03f;
        private const float MIN_DIFF_TO_MOVE = 0.001f;

        private List<UIMouseAllowEvent> _canMouseDownEvents;
        private List<UIMouseAllowEvent> _canMouseUpEvents;
        private List<UIMouseAllowEvent> _canMouseDragEvents;

        private FlexboxLayout _flexboxLayout;
        private Vector2 _lastDragPos;
        private Vector2 _averageDiff;
        private float _dragDistance;
        private float _lastMagnetizationLength;

        protected override void AfterAwake()
        {
            _canMouseDownEvents = new List<UIMouseAllowEvent>();
            _canMouseUpEvents = new List<UIMouseAllowEvent>();
            _canMouseDragEvents = new List<UIMouseAllowEvent>();

            IsPressed = false;
            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);

            base.AfterAwake();
        }

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            if (props == null)
                throw new ArgumentNullException(nameof(props));

            if (!(props is ScrollViewProps viewSettings))
                throw new ArgumentException("ScrollView: The given settings is not ScrollViewSettings");

            if (viewSettings.FlexboxProps == null)
                throw new ArgumentException("ScrollView: FlexboxProps is null");

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddDragable(this);

            base.ApplyProps(props, parent);

            viewSettings.FlexboxProps.SortingOrder =
                Mathf.Max(1, viewSettings.FlexboxProps.SortingOrder);

            _flexboxLayout = Create<FlexboxLayout>(viewSettings.FlexboxProps, this);
            _flexboxLayout.ChildAdded += FlexboxLayoutOnChildAdded;

            if (viewSettings.FlexboxProps.Direction == FlexboxDirection.COLUMN)
            {
                _flexboxLayout.Anchor = UIAnchor.UpperCenter;
                _flexboxLayout.Position = this.Position(UIAnchor.UpperCenter);
            }
            else if (viewSettings.FlexboxProps.Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                _flexboxLayout.Anchor = UIAnchor.LowerCenter;
                _flexboxLayout.Position = this.Position(UIAnchor.LowerCenter);
            }
            else if (viewSettings.FlexboxProps.Direction == FlexboxDirection.ROW)
            {
                _flexboxLayout.Anchor = UIAnchor.MiddleLeft;
                _flexboxLayout.Position = this.Position(UIAnchor.MiddleLeft);
            }
            else if (viewSettings.FlexboxProps.Direction == FlexboxDirection.ROW_REVERSE)
            {
                _flexboxLayout.Anchor = UIAnchor.MiddleRight;
                _flexboxLayout.Position = this.Position(UIAnchor.MiddleRight);
            }
        }

        private void OnUIMouseUp(Vector2 worldPos)
        {
            _lastDragPos = worldPos;
            _averageDiff *= 10f;
            _dragDistance = 0f;
        }

        private void OnUIMouseDown(Vector2 worldPos)
        {
            var sliderRect = Rect;
            var itemsRect = _flexboxLayout.Rect;

            _dragDistance = 0f;
            _lastDragPos = worldPos;
            _averageDiff = Vector2.zero;
            _lastMagnetizationLength = GetNormilizedMagnetization(sliderRect, 
                itemsRect, out _, out _, out _);
        }

        private void OnUIMouseDrag(Vector2 worldPos)
        {
            var sliderRect = Rect;
            var point1 = mMath.ClosestPointOnLine(sliderRect.Top, sliderRect.Bottom, _lastDragPos);
            var point2 = mMath.ClosestPointOnLine(sliderRect.Top, sliderRect.Bottom, worldPos);
            var shift = point2 - point1;

            _averageDiff = (_averageDiff + shift) / 2f;
            Move(sliderRect, shift);

            _lastDragPos = worldPos;
            _dragDistance += shift.Length();
        }

        protected override void OnTick()
        {
            if (!IsActive || IsPressed)
                return;

            var sliderRect = Rect;
            var itemsRect = _flexboxLayout.Rect;

            var magnetizationLength = GetNormilizedMagnetization(sliderRect, itemsRect,
                out var point1InArea, out var point2InArea, out var magnetization);

            if (!point1InArea && !point2InArea)
            {
                if (_averageDiff.Length() >= MIN_DIFF_TO_MOVE)
                {
                    _flexboxLayout.Translate(_averageDiff * Time.deltaTime * 5f);
                    _averageDiff *= 0.92f;
                }

                return;
            }

            if (magnetizationLength >= 0.999f)
                return;

            _flexboxLayout.Translate(magnetization * Time.deltaTime * 5f);
        }

        private float GetNormilizedMagnetization(UIRect sliderRect, UIRect itemsRect,
            out bool point1InArea, out bool point2InArea, out Vector2 magnetization)
        {
            if (_flexboxLayout.Direction == FlexboxDirection.COLUMN ||
                _flexboxLayout.Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                point1InArea = RectangleAreaChecker.InUIRect(sliderRect, itemsRect.Top);
                point2InArea = RectangleAreaChecker.InUIRect(sliderRect, itemsRect.Bottom);
            }
            else
            {
                point1InArea = RectangleAreaChecker.InUIRect(sliderRect, itemsRect.Left);
                point2InArea = RectangleAreaChecker.InUIRect(sliderRect, itemsRect.Right);
            }

            if (_flexboxLayout.Direction == FlexboxDirection.COLUMN ||
                _flexboxLayout.Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                if (point1InArea)
                    magnetization = sliderRect.Top - itemsRect.Top;
                else
                    magnetization = sliderRect.Bottom - itemsRect.Bottom;
            }
            else
            {
                if (point1InArea)
                    magnetization = sliderRect.Left - itemsRect.Left;
                else
                    magnetization = sliderRect.Right - itemsRect.Right;
            }

            var maxMagnetization =
                _flexboxLayout.Direction == FlexboxDirection.COLUMN ||
                _flexboxLayout.Direction == FlexboxDirection.COLUMN_REVERSE
                    ? Height / 4f
                    : Width / 4f;

            return 1f - magnetization.Length() / maxMagnetization;
        }

        private void Move(UIRect sliderRect, Vector2 shift)
        {
            var translatedItemRect = _flexboxLayout.GetRect(_flexboxLayout.TranslatedPos(shift));
            var magnetizationLength = GetNormilizedMagnetization(sliderRect, translatedItemRect,
                out var point1InArea, out var point2InArea, out var magnetization);

            if (!point1InArea && !point2InArea)
            {
                _flexboxLayout.Translate(shift);
                return;
            }

            if (magnetizationLength >= 0 && magnetizationLength <= _lastMagnetizationLength)
            {
                _flexboxLayout.Translate(shift * magnetizationLength);
                _lastMagnetizationLength = magnetizationLength;
            }
            else if (magnetizationLength > _lastMagnetizationLength)
            {
                _flexboxLayout.Translate(shift);
                _lastMagnetizationLength = magnetizationLength;
            }
        }

        public void DoMouseDrag(Vector2 worldPos)
        {
            if (!IsPressed)
                return;

            if (_canMouseDragEvents.Count != 0 &&
                !_canMouseDragEvents.TrueForAll(e => e(this, ref worldPos)))
            {
                return;
            }

            OnUIMouseDrag(worldPos);
            MouseDrag?.Invoke(this, worldPos);
        }

        public void DoMouseDown(Vector2 worldPos)
        {
            if (!IsActive || IsPressed || !AreaChecker.InAreaShape(this, worldPos))
                return;

            if (_canMouseDownEvents.Count != 0 &&
                !_canMouseDownEvents.TrueForAll(e => e(this, ref worldPos)))
            {
                return;
            }

            IsPressed = true;
            OnUIMouseDown(worldPos);
            MouseDown?.Invoke(this, worldPos);
        }

        public void DoMouseUp(Vector2 worldPos)
        {
            if (!IsPressed)
                return;

            if (_canMouseUpEvents.Count != 0 &&
                !_canMouseUpEvents.TrueForAll(e => e(this, ref worldPos)))
            {
                return;
            }

            OnUIMouseUp(worldPos);
            MouseUp?.Invoke(this, worldPos);
            IsPressed = false;
        }
        
        private void FlexboxLayoutOnChildAdded(IUIObject sender, IUIObject child)
        {
            if (child is IUIClickable clickable)
            {
                clickable.CanMouseUp += ChildClickableOnCanMouseUp;
                clickable.CanMouseDown += ChildClickableOnCanMouseDown;
            }

            child.ChildAdded += FlexboxLayoutOnChildAdded;
        }

        private bool ChildClickableOnCanMouseDown(IUIClickable sender, ref Vector2 worldPos)
        {
            return _dragDistance < MAX_PATH_TO_CLICK && AreaChecker.InAreaShape(this, worldPos);
        }

        private bool ChildClickableOnCanMouseUp(IUIClickable sender, ref Vector2 worldPos)
        {
            if (_dragDistance > MAX_PATH_TO_CLICK)
                worldPos = new Vector2(float.MinValue, float.MaxValue);
            return true;
        }

        public override UIView View(Type viewType, UIViewProps props, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(props, _flexboxLayout, @params);
            return view;
        }

        public override T Component<T>(UIComponentProps props)
        {
            return _flexboxLayout.Component<T>(props);
        }

        protected override void CreateInterface(object[] @params)
        {
        }
    }
}