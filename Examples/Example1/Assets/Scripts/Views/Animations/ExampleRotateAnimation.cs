using mFramework;
using mFramework.UI;

namespace Example.Animations
{
    public class ExampleRotateAnimation : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("level_info")})
                .Scale(0.3f)
                .PosY(this.RelativeY(0.30f))
                .RotateAnimation(new UIRotateAnimationSettings
                {
                    FromAngle = 0f,
                    ToAngle = 360,
                    EasingType = EasingType.easeOutBack,
                    Duration = 1f,
                    PlayType = UIAnimationPlayType.END_RESET
                });

            this.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("level_info") })
                .Scale(0.3f)
                .PosY(this.RelativeY(0.70f))
                .RotateAnimation(new UIRotateAnimationSettings
                {
                    FromAngle = 180f,
                    ToAngle = 90f,
                    EasingType = EasingType.easeOutBack,
                    Duration = 1f,
                    PlayType = UIAnimationPlayType.END_FLIP
                });

            base.CreateInterface(@params);
        }
    }
}