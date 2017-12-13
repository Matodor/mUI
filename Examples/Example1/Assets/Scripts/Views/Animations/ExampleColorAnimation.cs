using mFramework;
using mFramework.UI;

namespace Example.Animations
{
    public class ExampleColorAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("x_bg")})
                .ColorAnimation(new UIColorAnimationSettings
                {
                    FromColor = new UIColor(0f, 255, 255, 255f, UIColorType.HSVA),
                    ToColor = new UIColor(360f, 255, 255, 255f, UIColorType.HSVA),
                    Duration = 3f,
                    PlayType = UIAnimationPlayType.END_FLIP,
                    EasingType = EasingType.easeOutBounce,
                }).AnimatedObject.PosY(this.RelativeY(0.3f));

            this.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("x_bg") })
                .ColorAnimation(new UIColorAnimationSettings
                {
                    FromColor = new UIColor("#ffffff", 255),
                    ToColor = new UIColor("#000000", 150),
                    Duration = 3f,
                    PlayType = UIAnimationPlayType.END_FLIP,
                    EasingType = EasingType.linear,
                }).AnimatedObject.PosY(this.RelativeY(0.7f));

            base.CreateInterface(@params);
        }
    }
}