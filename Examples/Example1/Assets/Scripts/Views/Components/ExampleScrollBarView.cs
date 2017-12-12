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
            var scrollBar1 = CreateScrollBar(true, 10);
            scrollBar1.Translate(0f, +GetHeight() / 4);

            var scrollBar2 = CreateScrollBar(false, 0.2f);
            scrollBar2.Translate(0f, -GetHeight() / 4);

            CreateScrollBarLabel(scrollBar1);
            CreateScrollBarLabel(scrollBar2);
            base.CreateInterface(@params);
        }

        private UILabel CreateScrollBarLabel(UIBaseScrollBar scrollBar)
        {
            var label = scrollBar.Label(new UILabelSettings
            {
                Text = $"v={scrollBar.Value} vNormilized={scrollBar.NormilizedValue}",
                Color = UIColors.Black,
                TextAnchor = TextAnchor.MiddleCenter,
                TextAlignment = TextAlignment.Center,
                Size = 40,
            });
            label.SortingOrder(3);
            scrollBar.Changed += _ => label.SetText($"v={scrollBar.Value} vNormilized={scrollBar.NormilizedValue}");
            return label;
        }

        private UIBaseScrollBar CreateScrollBar(bool vertical, float step)
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
                Step = step,
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
                    vertical ? 0f : Game.GetSprite("x_bg").WorldSize().x / 2f * 1.39576f,
                    vertical ? Game.GetSprite("x_bg").WorldSize().y / 2f * 1.39576f : 0f
                ),
                ScalePoint = new Vector2(1.39576f, 1.39576f)
            }).Scale(0.5f);
        }
    }
}