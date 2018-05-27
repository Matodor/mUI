using System;
using System.Collections.Generic;
using System.Linq;

namespace mFramework.UI
{
    public class LambdaAnimationSettings : UIAnimationSettings
    {
        public Action<UIAnimation> OnAnimate;
    }

    public static partial class UIExtensions
    {
        public static LambdaAnimation[] LambdaAnimation(
            this IEnumerable<IUIObject> objs, LambdaAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<LambdaAnimation>(settings)).ToArray();
        }

        public static LambdaAnimation LambdaAnimation(this IUIObject obj,
            LambdaAnimationSettings settings)
        {
            return obj.Animation<LambdaAnimation>(settings);
        }
    }

    public class LambdaAnimation : UIAnimation
    {
        private Action<UIAnimation> _onAnimate;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is LambdaAnimationSettings lambdaSettings))
                throw new ArgumentException("LambdaAnimation: The given settings is not LambdaAnimationSettings");

            _onAnimate = lambdaSettings.OnAnimate;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            _onAnimate(this);
        }
    }
}