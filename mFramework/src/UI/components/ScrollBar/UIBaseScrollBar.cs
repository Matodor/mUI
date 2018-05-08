using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIScrollBarSettings : UIComponentSettings
    {
        public Sprite BarSprite;
        public readonly UIButtonSettings BarPointSettings = new UIButtonSettings();

        public Vector2 ScaleBar = Vector2.one;
        public Vector2 ScalePoint = Vector2.one;
        public bool BarSpriteIsHorizontal = true;

        public Vector2 Padding = Vector2.zero;
        public float Min = 0f;
        public float Max = 1f;
        public float Default = 0.5f;
        public float Step = 0.2f;
    }

    public abstract class UIBaseScrollBar : UIComponent, IUIClickableOld
    {
        public event UIEventHandler<UIBaseScrollBar> Changed = delegate { };

        public float Value
        {
            get => _value;
            set
            {
                _normilizedValue = mMath.Clamp(mMath.NormilizeValue(Min, Max, value), 0f, 1f);
                UpdateBar();
            }
        }

        public float NormilizedValue
        {
            get => _normilizedValue;
            set
            {
                _normilizedValue = mMath.Clamp(value, 0f, 1f);
                UpdateBar();
            }
        }

        public float Min { get; private set; }
        public float Max { get; private set; }

        public Vector2 Padding;
        public float Step;
        public float NormilizedStep;

        public UISprite Bar { get; private set; }
        public UIButton BarPoint { get; private set; }
        public UIClickableOld UiClickableOld { get; private set; }

        protected bool BarSpriteIsHorizontal { get; private set; }

        private float _normilizedValue;
        private float _value;

        private bool _isPressed;

        protected override void AfterAwake()
        {
            _isPressed = false;
            base.AfterAwake();
        }
        
        public static float NormilizeStep(float min, float max, float step)
        {
            return step / (max - min);
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIScrollBarSettings scrollBarSettings))
                throw new ArgumentException("UIBaseScrollBar: The given settings is not UIScrollBarSettings");

            BarSpriteIsHorizontal = scrollBarSettings.BarSpriteIsHorizontal;
            Padding = scrollBarSettings.Padding;

            Min = Mathf.Min(scrollBarSettings.Min, scrollBarSettings.Max);
            Max = Mathf.Max(scrollBarSettings.Min, scrollBarSettings.Max);

            Step = scrollBarSettings.Step;
            NormilizedStep = NormilizeStep(Min, Max, Step);

            Bar = this.Sprite(new UISpriteSettings {Sprite = scrollBarSettings.BarSprite});
            Bar.Scale = scrollBarSettings.ScaleBar;

            BarPoint = Bar.Button(scrollBarSettings.BarPointSettings);
            BarPoint.Scale = scrollBarSettings.ScalePoint;
            BarPoint.SortingOrder(1);

            var area = new RectangleArea2D();
            area.Update += a =>
            {
                area.Width = Width;
                area.Height = Height;
            };
            UiClickableOld = new UIClickableOld(this, area);

            _value = scrollBarSettings.Default;
            _normilizedValue = mMath.NormilizeValue(Min, Max, scrollBarSettings.Default);

            UpdateBar();
            base.ApplySettings(settings);
        }

        private void UpdateBar()
        {
            var v = _normilizedValue / NormilizedStep;
            var c = (float) Math.Truncate(v);
            var o = v - c;

            if (o < 0.5f)
                _normilizedValue = NormilizedStep * c;
            else
                _normilizedValue = NormilizedStep * c + NormilizedStep;
            _normilizedValue = mMath.Clamp(_normilizedValue, 0f, 1f);

            var prevValue = _value;
            _value = BezierHelper.Linear(_normilizedValue, Min, Max);
            UpdateBar(_normilizedValue);
            Changed(this);
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            MovePoint(worldPos);
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            _isPressed = false;
        }

        protected abstract void MovePoint(Vector2 newPos);
        protected abstract void UpdateBar(float normilized);

        public void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            MovePoint(worldPos);
        }
    }
}