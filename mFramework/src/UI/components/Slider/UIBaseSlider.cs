using System;
using System.Collections.Generic;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class UISliderSettings : UIComponentSettings
    {
        public float? Height = null;
        public float? Width = null;
        public float Offset = 0;
        public float TimeToStop = 0.3f;

        public LayoutElemsDirection ElementsDirection = LayoutElemsDirection.FORWARD;
        public ushort StencilId = 1 << 2;
    }

    public abstract class UIBaseSlider : UIComponent, IUIClickable, IView
    {
        public const float SLIDER_MAX_PATH_TO_CLICK = 0.03f;
        public const float SLIDER_MIN_DIFF_TO_MOVE = 0.0001f;

        public ushort? StencilId => _stencilId;
        public UIClickable UIClickable { get; private set; }

        protected LayoutElemsDirection ElementsDirection { get; private set; }
        protected bool IsPressed { get; private set; }
        protected float ElementsOffset { get; private set; }

        public float TimeToStop;

        private ushort _stencilId;
        private float _height;
        private float _width;

        private Vector2 _lastMousePos;
        private Vector2 _lastMoveDiff;
        private Vector2 _dragPath;
        private List<Pair<IUIClickable, Vector2>> _clickNext;
        private DateTime _mouseUpAt;

        protected abstract void OnChildObjectAdded(IUIObject sender, IUIObject child);
        protected abstract void SliderDrag(Vector2 drag);

        protected override void Init()
        {
            IsPressed = false;
            TimeToStop = 0.3f;

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

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISliderSettings sliderSettings))
                throw new ArgumentException("UISlider: The given settings is not UIComponentSettings");

            ElementsDirection = sliderSettings.ElementsDirection;
            ElementsOffset = sliderSettings.Offset;
            TimeToStop = sliderSettings.TimeToStop;

            _stencilId = sliderSettings.StencilId;
            _height = sliderSettings.Height ?? ParentView.GetHeight();
            _width = sliderSettings.Width ?? ParentView.GetWidth();

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
            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            if (UIClickable.Area2D.InArea(worldPos) &&
                _dragPath.Length() < SLIDER_MAX_PATH_TO_CLICK)
            {
                _clickNext.Add(new Pair<IUIClickable, Vector2>(sender, worldPos));
                return true;
            }
            return false;
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
            IsPressed = true;
            _lastMousePos = worldPos;
        }

        public void MouseUp(Vector2 worldPos)
        {
            IsPressed = false;
            _lastMousePos = worldPos;
            _mouseUpAt = DateTime.Now;

            if (_clickNext.Count > 0)
            {
                if (_dragPath.Length() < SLIDER_MAX_PATH_TO_CLICK)
                    _clickNext.ForEach(e => e.First.MouseUp(e.Second));
                else
                    _clickNext.ForEach(e => e.First.MouseUp(new Vector2(float.MinValue, float.MinValue))); // fake pos
                _clickNext.Clear();
            }

            _dragPath = Vector2.zero;
        }

        public void MouseDrag(Vector2 worldPos)
        {
            if (!IsPressed)
                return;
             
            _lastMoveDiff = worldPos - _lastMousePos;
            _dragPath += _lastMoveDiff;

            SliderDrag(_lastMoveDiff);
            _lastMousePos = worldPos;
        }

        protected abstract bool CheckInnerSpace();

        protected override void OnTick()
        {
            if (!IsActive || IsPressed || CheckInnerSpace())
            {
                base.OnTick();
                return;
            }

            var t = (float)(DateTime.Now - _mouseUpAt).TotalSeconds / TimeToStop;
            t = 1 - t;

            if (t <= 0f)
            {
                return;
            }

            SliderDrag(_lastMoveDiff * t);
            base.OnTick();
        }

        public float RelativeX(float t)
        {
            return Position().x - GetWidth() / 2 + GetWidth() * mMath.Clamp(t, 0, 1);
        }

        public float RelativeY(float t)
        {
            return Position().y - GetHeight() / 2 + GetHeight() * mMath.Clamp(t, 0, 1);
        }
    }
}