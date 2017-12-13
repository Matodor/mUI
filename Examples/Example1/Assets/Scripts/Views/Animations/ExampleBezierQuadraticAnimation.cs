using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example.Animations
{
    public class ExampleBezierQuadraticAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("x_bg")})
                .BezierQuadraticAnimation(new UIBezierQuadraticAnimationSettings
                {
                    FirstPoint = new Vector2(this.RelativeX(0f), this.RelativeY(0.5f)),
                    SecondPoint = new Vector2(this.RelativeX(0.5f), this.RelativeY(1f)),
                    ThirdPoint = new Vector2(this.RelativeX(1f), this.RelativeY(0.5f)),
                    EasingType = EasingType.easeInOutBounce,
                    Duration = 5f,
                    PlayType = UIAnimationPlayType.END_RESET,
                });

            this.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("x_bg") })
                .SetColor(Color.green)
                .Scale(0.5f)
                .BezierQuadraticAnimation(new UIBezierQuadraticAnimationSettings
                {
                    FirstPoint = new Vector2(this.RelativeX(0.5f), this.RelativeY(0.5f)),
                    SecondPoint = new Vector2(this.RelativeX(0f), this.RelativeY(1f)),
                    ThirdPoint = new Vector2(this.RelativeX(0.4f), this.RelativeY(0f)),
                    EasingType = EasingType.easeInQuint,
                    Duration = 5f,
                    PlayType = UIAnimationPlayType.END_FLIP,
                });

            base.CreateInterface(@params);
        }
    }
}