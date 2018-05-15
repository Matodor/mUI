using System;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class ScrollViewSettings : UIViewSettings
    {
        public virtual FlexboxLayoutSettings FlexboxSettings { get; set; }
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
        private const float MIN_DIFF_TO_MOVE = 0.0001f;

        private FlexboxLayout _flexboxLayout;
        private Vector2 _lastMousePos;
        private Vector2 _lastMoveDiff;
        private float _dragDistance;

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
            return !IsPressed && AreaChecker.InAreaShape(this, worldPos);
        }

        private void OnUIMouseUp(IUIClickable sender, ref Vector2 worldPos)
        {
            _lastMousePos = worldPos;
        }

        private void OnUIMouseDown(IUIClickable sender, ref Vector2 worldPos)
        {
            _dragDistance = 0f;
            _lastMousePos = worldPos;
            _lastMoveDiff = Vector2.zero;
        }

        private void OnUIMouseDrag(IUIClickable sender, ref Vector2 worldPos)
        {
            var shift = worldPos - _lastMousePos;
            _lastMousePos = worldPos;
            _dragDistance += _lastMoveDiff.Length();

            Move(shift);
        }

        public void Move(Vector2 shift)
        {
            var sliderRect = Rect;
            var itemsRect = _flexboxLayout.Rect;

            // horizontal
            if (_flexboxLayout.Direction == FlexboxDirection.ROW ||
                _flexboxLayout.Direction == FlexboxDirection.ROW_REVERSE)
            {
                shift.y = 0;
            }
            // vertical
            else
            {
                shift.x = 0f;
            }

            _flexboxLayout.Translate(shift);
            _lastMoveDiff = shift;
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

        protected override void ApplySettings(UIViewSettings settings, IView parent)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is ScrollViewSettings viewSettings))
                throw new ArgumentException("ScrollView: The given settings is not ScrollViewSettings");

            _flexboxLayout = Create<FlexboxLayout>(viewSettings.FlexboxSettings, this);
            _flexboxLayout.ChildObjectAdded += FlexboxLayoutOnChildObjectAdded;
            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddDragable(this);

            base.ApplySettings(settings, parent);
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

        public override UIView View(Type viewType, UIViewSettings settings, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, _flexboxLayout, @params);
            return view;
        }

        public override T Component<T>(UIComponentSettings settings)
        {
            return _flexboxLayout.Component<T>(settings);
        }

        protected override void CreateInterface(object[] @params)
        {
        }
    }
}