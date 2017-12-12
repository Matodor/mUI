using System;
using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleScrollBarView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            CreateScrollBar(true).Translate(0f, +GetHeight() / 4);
            CreateScrollBar(false).Translate(0f, -GetHeight() / 4);
            base.CreateInterface(@params);
        }

        private UIBaseScrollBar CreateScrollBar(bool vertical)
        {
            Func<bool, UIScrollBarSettings, UIBaseScrollBar> createFunc = (isVertical, settings) =>
            {
                if (isVertical)
                    return this.VerticalScrollBar(settings);
                return this.HorizontalScrollBar(settings);
            };

            return (UIBaseScrollBar) createFunc(vertical, new UIScrollBarSettings
            {
                Min = 100,
                Max = 200,
                Step = 2,
                Default = 150,
                BarSprite = Game.GetSprite("level_info"),
                BarSpriteIsHorizontal = true,
                BarPointSettings =
                {
                    ButtonSpriteStates =
                    {
                        Default = Game.GetSprite("x_bg"),
                        Highlighted = Game.GetSprite("x_bg_pressed")
                    }
                },
                Padding = new Vector2(
                    vertical ? 0f : Game.GetSprite("x_bg").WorldSize().x * 1.39576f,
                    vertical ? Game.GetSprite("x_bg").WorldSize().y * 1.39576f : 0f
                ),
                ScalePoint = new Vector2(1.39576f, 1.39576f)
            }).Scale(0.5f);
        }
    }
}