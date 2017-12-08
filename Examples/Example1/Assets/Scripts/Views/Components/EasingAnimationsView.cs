using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class EasingAnimationsView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var slider = this.VerticalSlider(new UISliderSettings
            {
                Height = GetHeight() * 0.8f
            });

            var sp = Game.GetSprite("mp_chat_msg");

            for (var i = EasingType.linear; i <= EasingType.easeInOutBounce; i++)
            {
                var c = slider.Container(new UIContainerSettings
                {
                    Width = GetWidth(),
                    Height = sp.WorldSize().y * 2
                }).SortingOrder(1);

                c.Label(new UILabelSettings
                {
                    TextAlignment = TextAlignment.Left,
                    TextAnchor = TextAnchor.UpperLeft,
                    Text = i.ToString(),
                    Size = 20,
                }).Translate(-c.GetWidth() / 2, 0);

                var sprite = c.Sprite(new UISpriteSettings
                {
                    Sprite = sp
                });
                sprite.Translate(-sprite.Parent.GetWidth() / 2 + sprite.GetWidth() / 2, -sprite.GetHeight());
                sprite.LinearAnimation(new UILinearAnimationSettings
                {
                    StartPos = sprite.LocalPos(),
                    EndPos = new Vector2(
                        sprite.Parent.GetWidth() / 2 - sprite.GetWidth() / 2,
                        sprite.LocalPos().y
                    ),
                    EasingType = i,
                    Duration = 4f,
                    PlayType = UIAnimationPlayType.END_RESET,
                    LocalPosition = true
                });
            }

            base.CreateInterface(@params);
        }
    }
}