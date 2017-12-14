using System;
using System.Collections.Generic;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class UIPageSliderSettings : UIComponentSettings
    {
        public float? Height = null;
        public float? Width = null;
        public float Duration = 0.5f;

        public EasingType EasingCurrentPageType = EasingType.linear;
        public EasingType EasingNextPageType = EasingType.linear;
        public LayoutElemsDirection ElementsDirection = LayoutElemsDirection.FORWARD;
        public ushort StencilId = 1 << 2;
    }

    public abstract class UIBasePageSlider : UIComponent, IUIClickable, IView
    {
        public event UIEventHandler<UIBasePageSlider, IUIObject, IUIObject> BeforePageChange = delegate { };
        public event UIEventHandler<UIBasePageSlider, IUIObject, IUIObject> PageChanged = delegate { };
        public event Func<UIBasePageSlider, bool> CanChangePage = delegate { return true; };

        public ushort? StencilId => _stencilId;
        public UIClickable UIClickable { get; private set; }
        public LayoutElemsDirection ElementsDirection { get; private set; }

        public int CurrentPage { get; protected set; }
        protected bool IsAnimated;
        protected float Duration;
        protected EasingType EasingCurrentPageType;
        protected EasingType EasingNextPageType;

        public abstract bool MoveNext();
        public abstract bool MovePrev();

        protected abstract void OnChildObjectAdded(IUIObject sender, IUIObject child);
        protected abstract void SliderDrag(Vector2 drag);

        private ushort _stencilId;
        private float _height;
        private float _width;

        private bool _isPressed;
        private Vector2 _lastMousePos;
        private Vector2 _lastMoveDiff;
        private Vector2 _dragPath;
        private List<Pair<IUIClickable, Vector2>> _clickNext;

        protected bool BeforeMove()
        {
            return CanChangePage(this);
        }

        protected void OnBeforePageChange(IUIObject prev, IUIObject next)
        {
            BeforePageChange(this, prev, next);
        }

        protected void OnPageChanged(IUIObject prev, IUIObject next)
        {
            PageChanged(this, prev, next);
        }

        protected override void Init()
        {
            IsAnimated = false;
            CurrentPage = -1;

            _isPressed = false;
            _lastMousePos = Vector2.zero;
            _lastMoveDiff = Vector2.zero;
            _dragPath = Vector2.zero;
            _clickNext = new List<Pair<IUIClickable, Vector2>>();

            ChildObjectAdded += OnChildObjectAdded;
            ChildObjectAdded += SetupChilds;

            BeforeDestroy += _ =>
            {
                ChildObjectAdded -= OnChildObjectAdded;
                ChildObjectAdded -= SetupChilds;
            };
            base.Init();
        }

        private void SetupChilds(IUIObject sender, IUIObject addedObj)
        {
            if (addedObj is IUIClickable uiClickable)
            {
                uiClickable.UIClickable.CanMouseDown += CanChildsMouseDown;
                uiClickable.UIClickable.CanMouseUp += CanChildsMouseUp;
            }

            addedObj.ChildObjectAdded += SetupChilds;
        }

        private bool CanChildsMouseUp(IUIClickable sender, MouseEvent e)
        {
            return false;
        }

        private bool CanChildsMouseDown(IUIClickable sender, MouseEvent e)
        {
            if (IsAnimated)
                return false;

            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            if (UIClickable.Area2D.InArea(worldPos) &&
                _dragPath.Length() < UIBaseSlider.SLIDER_MAX_PATH_TO_CLICK)
            {
                _clickNext.Add(new Pair<IUIClickable, Vector2>(sender, worldPos));
                return true;
            }
            return false;
        }

        protected UILinearAnimation Animate(IUIObject obj, bool isVertical, float translate, EasingType easingType)
        {
            return obj.LinearAnimation(new UILinearAnimationSettings
            {
                LocalPosition = true,
                StartPos = obj.LocalPos(),
                EndPos = isVertical ? obj.LocalTranslatedY(translate) : obj.LocalTranslatedX(translate),
                PlayType = UIAnimationPlayType.PLAY_ONCE,
                Duration = Duration,
                EasingType = easingType
            });
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIPageSliderSettings pageSliderSettings))
                throw new ArgumentException("UIBasePageSlider: The given settings is not UIPageSliderSettings");

            Duration = pageSliderSettings.Duration;
            EasingCurrentPageType = pageSliderSettings.EasingCurrentPageType;
            EasingNextPageType = pageSliderSettings.EasingNextPageType;
            ElementsDirection = pageSliderSettings.ElementsDirection;
            
            _stencilId = pageSliderSettings.StencilId;
            _height = pageSliderSettings.Height ?? ParentView.GetHeight();
            _width = pageSliderSettings.Width ?? ParentView.GetWidth();

            var area = new RectangleArea2D();
            area.Update += a =>
            {
                area.Width = GetWidth();
                area.Height = GetHeight();
            };
            UIClickable = new UIClickable(this, area);

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(_stencilId).CanvasMaterial;
            meshRenderer.sharedMaterial.SetTexture("_MainTex", Texture2D.blackTexture);
            meshRenderer.sortingOrder = SortingOrder();
            UIStencilMaterials.CreateMesh(this);

            SortingOrderChanged += sender =>
            {
                meshRenderer.sortingOrder = SortingOrder();
            };

            base.ApplySettings(settings);
        }

        public override float UnscaledHeight()
        {
            return _height;
        }

        public override float UnscaledWidth()
        {
            return _width;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            _lastMousePos = worldPos;
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            _isPressed = false;
            _lastMousePos = worldPos;

            if (_clickNext.Count > 0)
            {
                if (_dragPath.Length() < UIBaseSlider.SLIDER_MAX_PATH_TO_CLICK)
                    _clickNext.ForEach(e => e.First.MouseUp(e.Second));
                else
                    _clickNext.ForEach(e => e.First.MouseUp(new Vector2(float.MinValue, float.MinValue))); // fake pos
                _clickNext.Clear();
            }

            _dragPath = Vector2.zero;
        }

        public void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            _lastMoveDiff = worldPos - _lastMousePos;
            _dragPath += _lastMoveDiff;

            SliderDrag(_lastMoveDiff);
            _lastMousePos = worldPos;
        }
    }
}
 
 