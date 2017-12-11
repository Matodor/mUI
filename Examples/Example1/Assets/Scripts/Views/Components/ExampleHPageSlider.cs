﻿using mFramework;
using mFramework.UI;
using mFramework.UI.Layouts;
using mFramework.UI.PageSlider;

namespace Example
{
    public class ExampleHPageSlider : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var pageSlider = this.HorizontalPageSlider(new UIPageSliderSettings
            {
                ElementsDirection = LayoutElemsDirection.FORWARD,
                EasingCurrentPageType = EasingType.easeInCubic,
                EasingNextPageType = EasingType.easeOutCubic,
                Duration = 2f
            });

            for (int i = 0; i < 5; i++)
                pageSlider.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("mp_bar_buttons") }).SortingOrder(1);

            base.CreateInterface(@params);
        }
    }
}