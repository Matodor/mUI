using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example.Examples
{
    public class UIButtonExample : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var button = this.Button(new UIButtonSettings
            {
                Sprite = Game.GetSprite("mp_bar_text"),
            });

            button.ShiftAnimation(new UIShiftAnimationSettings
            {
                Shift = new Vector3(button.Width, 0),
                Duration = 0.2f,
                PlayType = UIAnimationPlayType.END_FLIP,
                EasingType = EasingType.easeInOutBack,
                MaxRepeats = 500
            });

            button.ShiftAnimation(new UIShiftAnimationSettings
            {
                Shift = new Vector3(0, button.Height),
                Duration = 3,
                PlayType = UIAnimationPlayType.END_FLIP,
                EasingType = EasingType.easeInOutBack,
            });

            base.CreateInterface(@params);
        }
    }
}