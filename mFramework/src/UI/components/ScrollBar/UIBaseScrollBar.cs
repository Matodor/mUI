using System;
using UnityEngine;

namespace mFramework.UI.ScrollBar
{
    public class UIScrollBarSettings : UIComponentSettings
    {
        public Sprite BarSprite;
        public Sprite PointSprite;

        public float Min = 0f;
        public float Max = 1f;
        public float Default = 0.5f;
        public float Step = 0.2f;
    }

    public abstract class UIBaseScrollBar : UIComponent, IUIClickable
    {
        public event UIEventHandler<UIBaseScrollBar> Changed = delegate { };

        public float Value
        {
            get => _value;
            set
            {
                _value = mMath.Clamp(value, _minValue, _maxValue);
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

        public float Step;
        public float NormilizedStep;

        public float Min => _minValue;
        public float Max => _maxValue;

        public UISprite Bar { get; private set; }
        public UISprite BarPoint { get; private set; }
        public UIClickable UIClickable { get; private set; }

        private float _normilizedValue;
        private float _value;

        private float _minValue;
        private float _maxValue;
        private bool _isPressed;

        protected override void Init()
        {
            _isPressed = false;
            base.Init();
        }

        private static float NormilizeValue(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        private static float NormilizeStep(float min, float max, float step)
        {
            return step / (max - min);
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIScrollBarSettings scrollBarSettings))
                throw new ArgumentException("UIBaseScrollBar: The given settings is not UIScrollBarSettings");

            _minValue = Mathf.Min(scrollBarSettings.Min, scrollBarSettings.Max);
            _maxValue = Mathf.Max(scrollBarSettings.Min, scrollBarSettings.Max);

            Step = scrollBarSettings.Step;
            NormilizedStep = NormilizeStep(_minValue, _maxValue, Step);

            Bar = this.Sprite(new UISpriteSettings {Sprite = scrollBarSettings.BarSprite});
            BarPoint = Bar.Sprite(new UISpriteSettings {Sprite = scrollBarSettings.PointSprite});
            BarPoint.SortingOrder(1);

            var area = new RectangleArea2D();
            area.Update += a =>
            {
                area.Width = GetWidth();
                area.Height = GetHeight();
            };

            UIClickable = new UIClickable(this, area);

            _value = scrollBarSettings.Default;
            _normilizedValue = NormilizeValue(_minValue, _maxValue, scrollBarSettings.Default);

            mCore.Log($"value={_value} _normilizedValue={_normilizedValue} step={Step} NormilizedStep={NormilizedStep}");
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
            _value = BezierHelper.Linear(_normilizedValue, _minValue, _maxValue);
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            _isPressed = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

        }
    }
}