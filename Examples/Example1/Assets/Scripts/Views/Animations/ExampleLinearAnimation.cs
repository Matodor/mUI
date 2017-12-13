using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example.Animations
{
    public class ExampleLinearAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("GreenCircle100px")})
                .LinearAnimation(new UILinearAnimationSettings
                {
                    StartPos = new Vector2(this.RelativeX(0.5f), this.RelativeY(1f)),
                    EndPos = new Vector2(this.RelativeX(0.5f), this.RelativeY(0f)),
                    EasingType = EasingType.easeOutBounce,
                    PlayType = UIAnimationPlayType.END_FLIP,
                    Duration = 3f,
                }).AnimationRepeat += s => s.EasingType = s.EasingType == EasingType.easeOutBounce
                    ? EasingType.easeInBounce
                    : EasingType.easeOutBounce;

            this.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("GreenCircle100px") })
                .LinearAnimation(new UILinearAnimationSettings
                {
                    StartPos = new Vector2(this.RelativeX(0f), this.RelativeY(0.5f)),
                    EndPos = new Vector2(this.RelativeX(1f), this.RelativeY(0.5f)),
                    EasingType = EasingType.easeInOutQuad,
                    PlayType = UIAnimationPlayType.END_RESET,
                    Duration = 5f,
                    DestroyUIObjectOnEnd = true,
                    MaxRepeats = 5
                });

            base.CreateInterface(@params);
        }
    }
}