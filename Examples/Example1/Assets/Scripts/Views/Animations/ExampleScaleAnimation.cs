using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example.Animations
{
    public class ExampleScaleAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("x_bg")})
                .PosY(this.RelativeY(0.3f))
                .ScaleAnimation(new UIScaleAnimationSettings
                {
                    StartScale = Vector2.one,
                    EndScale = Vector2.one * 3f,
                    EasingType = EasingType.easeInOutBounce,
                    Duration = 5f,
                    PlayType = UIAnimationPlayType.END_RESET
                });

            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("x_bg")})
                .PosY(this.RelativeY(0.7f))
                .ScaleAnimation(new UIScaleAnimationSettings
                {
                    StartScale = Vector2.one,
                    EndScale = Vector2.one * 2f,
                    EasingType = EasingType.easeOutQuart,
                    Duration = 5f,
                    PlayType = UIAnimationPlayType.END_FLIP
                });
            base.CreateInterface(@params);
        }
    }
}