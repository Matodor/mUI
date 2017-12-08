using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleButtonView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var button = this.Button(new UIButtonSettings
            {
                ButtonSpriteStates =
                {
                    Default = Game.GetSprite("notif_agree"),
                    Highlighted = Game.GetSprite("notif_agree_closed")
                }
            });
            
            button.Click += _ =>
            {
                Debug.Log("Click");
            };

            SetupScaleAnimation(button);
            base.CreateInterface(@params);
        }

        private static void SetupScaleAnimation(UIButton button)
        {
            const float SCALE = 1.15f;
            var scaleAnimation = (UIScaleAnimation) null;

            button.ButtonDown += (sender, vector2) =>
            {
                scaleAnimation?.Remove();
                scaleAnimation = sender.ScaleAnimation(new UIScaleAnimationSettings
                {
                    StartScale = Vector2.one,
                    EndScale = Vector2.one * SCALE,
                    EasingType = EasingType.easeOutBounce,
                    Duration = 0.5f,
                    PlayType = UIAnimationPlayType.PLAY_ONCE
                });
            };

            button.ButtonUp += (sender, vector2) =>
            {
                if (scaleAnimation == null)
                    return;

                var t2 = scaleAnimation?.CurrentTime ?? 0f;
                scaleAnimation?.Remove();
                scaleAnimation = sender.ScaleAnimation(new UIScaleAnimationSettings
                {
                    StartScale = Vector2.one * SCALE,
                    EndScale = Vector2.one,
                    EasingType = EasingType.easeOutBounce,
                    Duration = 0.5f,
                    PlayType = UIAnimationPlayType.PLAY_ONCE
                });
                scaleAnimation.SetAnimationPos(1 - t2);
                scaleAnimation.AnimationEnded += s => { scaleAnimation = null; };
            };
        }
    }
}