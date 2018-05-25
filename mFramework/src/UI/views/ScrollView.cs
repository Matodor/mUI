using System;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class ScrollViewProps : UIViewProps
    {
        public virtual FlexboxLayoutProps FlexboxProps { get; set; }
    }

    public class ScrollView : UIView, IUIDragable
    {
        public event UIMouseEvent MouseDown;
        public event UIMouseEvent MouseUp;
        public event UIMouseEvent MouseDrag;

        public event UIMouseAllowEvent CanMouseDown = delegate { return true; };
        public event UIMouseAllowEvent CanMouseUp = delegate { return true; };
        public event UIMouseAllowEvent CanMouseDrag = delegate { return true; };

        public bool IsPressed { get; protected set; }
        public IAreaChecker AreaChecker { get; set; }

        private const float MAX_PATH_TO_CLICK = 0.03f;
        private const float MIN_DIFF_TO_MOVE = 0.001f;

        private FlexboxLayout _flexboxLayout;
        private Vector2 _lastDragPos;
        private Vector2 _averageDiff;
        private float _dragDistance;
        private float _lastMagnetizationLength;

        protected override void AfterAwake()
        {
            MouseDown += OnUIMouseDown;
            MouseUp += OnUIMouseUp;
            MouseDrag += OnUIMouseDrag;

            CanMouseDown += OnUICanMouseDown;
            CanMouseUp += OnUICanMouseUp;
            CanMouseDrag += OnUICanMouseDrag;

            IsPressed = false;
            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);

            base.AfterAwake();
        }

        private bool OnUICanMouseDrag(IUIClickable sender, ref Vector2 worldPos)
        {
            return IsPressed;
        }

        private bool OnUICanMouseUp(IUIClickable sender, ref Vector2 worldPos)
        {
            return IsPressed;
        }

        private bool OnUICanMouseDown(IUIClickable sender, ref Vector2 worldPos)
        {
            return IsActive && !IsPressed && AreaChecker.InAreaShape(this, worldPos);
        }

        private void OnUIMouseUp(IUIClickable sender, ref Vector2 worldPos)
        {
            _lastDragPos = worldPos;
            _averageDiff *= 10f;
        }

        private void OnUIMouseDown(IUIClickable sender, ref Vector2 worldPos)
        {
            var sliderRect = Rect;
            var itemsRect = _flexboxLayout.Rect;

            _dragDistance = 0f;
            _lastDragPos = worldPos;
            _averageDiff = Vector2.zero;
            _lastMagnetizationLength = GetNormilizedMagnetization(sliderRect, 
                itemsRect, out _, out _, out _);
        }

        private void OnUIMouseDrag(IUIClickable sender, ref Vector2 worldPos)
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
            if (!CanMouseDrag(this, ref worldPos))
                return;

            // ReSharper disable once PossibleNullReferenceException
            MouseDrag(this, ref worldPos);
        }

        public void DoMouseDown(Vector2 worldPos)
        {
            if (!CanMouseDown(this, ref worldPos))
                return;

            IsPressed = true;
            // ReSharper disable once PossibleNullReferenceException
            MouseDown(this, ref worldPos);
        }

        public void DoMouseUp(Vector2 worldPos)
        {
            if (!CanMouseUp(this, ref worldPos))
                return;

            // ReSharper disable once PossibleNullReferenceException
            MouseUp(this, ref worldPos);
            IsPressed = false;
        }

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            if (props == null)
                throw new ArgumentNullException(nameof(props));

            if (!(props is ScrollViewProps viewSettings))
                throw new ArgumentException("ScrollView: The given settings is not ScrollViewSettings");

            _flexboxLayout = Create<FlexboxLayout>(viewSettings.FlexboxProps, this);
            _flexboxLayout.ChildObjectAdded += FlexboxLayoutOnChildObjectAdded;
            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddDragable(this);

            base.ApplyProps(props, parent);
        }

        private void FlexboxLayoutOnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            if (child is IUIClickable clickable)
            {
                clickable.CanMouseUp += ChildClickableOnCanMouseUp;
                clickable.CanMouseDown += ChildClickableOnCanMouseDown;
            }
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