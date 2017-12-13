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
                });


            base.CreateInterface(@params);
        }
    }
}