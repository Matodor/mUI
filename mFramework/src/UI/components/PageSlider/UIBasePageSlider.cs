using System;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI.PageSlider
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

    public abstract class UIBasePageSlider : UIBaseSlider
    {
        public event UIEventHandler<UIBasePageSlider> PageChanged = delegate { }; 

        protected int CurrentPage = -1;
        protected bool IsAnimated = false;
        protected float Duration;
        protected EasingType EasingCurrentPageType;
        protected EasingType EasingNextPageType;

        public abstract void MoveNext();
        public abstract void MovePrev();

        protected void OnPageChanged()
        {
            PageChanged(this);
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

            if (!(settings is UIPageSliderSettings sliderSettings))
                throw new ArgumentException("UIBasePageSlider: The given settings is not UIPageSliderSettings");

            Duration = sliderSettings.Duration;
            EasingCurrentPageType = sliderSettings.EasingCurrentPageType;
            EasingNextPageType = sliderSettings.EasingNextPageType;

            base.ApplySettings(new UISliderSettings
            {
                StencilId = sliderSettings.StencilId,
                Padding = 0f,
                ElementsDirection = sliderSettings.ElementsDirection,
                Width = sliderSettings.Width,
                Height = sliderSettings.Height,
                Offset = 0f,
                TimeToStop = 0f
            });
        }
    }
}