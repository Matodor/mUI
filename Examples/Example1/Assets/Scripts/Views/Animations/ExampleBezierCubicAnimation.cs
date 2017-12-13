using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example.Animations
{
    public class ExampleBezierCubicAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var sprite = this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("x_bg")})
                .SetColor(Color.green)
                .Scale(0.5f);

            sprite.BezierCubicAnimation(new UIBezierCubicAnimationSettings
            {
                FirstPoint = new Vector2(this.RelativeX(0f) + sprite.GetWidth() / 2, this.RelativeY(0.3f) + sprite.GetHeight() / 2),
                SecondPoint = new Vector2(this.RelativeX(2f), this.RelativeY(1f)),
                ThirdPoint = new Vector2(this.RelativeX(-1f), this.RelativeY(1f)),
                FourthPoint = new Vector2(this.RelativeX(1f) - sprite.GetWidth() / 2, this.RelativeY(0.3f) + sprite.GetHeight() / 2),
                Duration = 5f,
                EasingType = EasingType.easeInOutQuint,
                PlayType = UIAnimationPlayType.END_FLIP,
            });

            sprite.ScaleAnimation(new UIScaleAnimationSettings
            {
                StartScale = sprite.GlobalScale(),
                EndScale = sprite.GlobalScale() * 2f,
                EasingType = EasingType.easeInOutQuint,
                Duration = 5f / 2f,
                PlayType = UIAnimationPlayType.END_FLIP
            });

            base.CreateInterface(@params);
        }
    }
}