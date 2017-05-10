using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Other
{
    public enum mUIEasingType
    {
        linear = 0,
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeInOutCirc,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
    }

    public static class mUIEasingFunctions
    {
        // c - current value
        // t - current time (0..1)
        // d - duration
        // b - offset
        public static float GetValue(mUIEasingType easingType, float c, float t, float d, float b = 0)
        {
            switch (easingType)
            {
                case mUIEasingType.easeInQuad:
                    return c * (t /= d) * t + b;
                case mUIEasingType.easeOutQuad:
                    return -c * (t /= d) * (t - 2) + b;
                case mUIEasingType.easeInOutQuad:
                    if ((t /= d / 2) < 1) return c / 2 * t * t + b;
                    return -c / 2 * ((--t) * (t - 2) - 1) + b;
                case mUIEasingType.easeInCubic:
                    return c * (t /= d) * t * t + b;
                case mUIEasingType.easeOutCubic:
                    return c * ((t = t / d - 1) * t * t + 1) + b;
                case mUIEasingType.easeInOutCubic:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t + 2) + b;
                case mUIEasingType.easeInQuart:
                    return c * (t /= d) * t * t * t + b;
                case mUIEasingType.easeOutQuart:
                    return -c * ((t = t / d - 1) * t * t * t - 1) + b;
                case mUIEasingType.easeInOutQuart:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
                    return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
                case mUIEasingType.easeInQuint:
                    return c * (t /= d) * t * t * t * t + b;
                case mUIEasingType.easeOutQuint:
                    return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
                case mUIEasingType.easeInOutQuint:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
                case mUIEasingType.easeInSine:
                    return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
                case mUIEasingType.easeOutSine:
                    return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
                case mUIEasingType.easeInOutSine:
                    return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
                case mUIEasingType.easeInExpo:
                    return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
                case mUIEasingType.easeOutExpo:
                    return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
                case mUIEasingType.easeInOutExpo:
                    if (t == 0) return b;
                    if (t == d) return b + c;
                    if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
                    return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
                case mUIEasingType.easeInCirc:
                    return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
                case mUIEasingType.easeInOutCirc:
                    if ((t /= d / 2) < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
                    return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
                case mUIEasingType.easeInElastic:
                    {
                        double s = 1.70158;
                        double p = 0;
                        float a = c;
                        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        return -(a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p))) + b;
                    }
                case mUIEasingType.easeOutElastic:
                    {
                        double s = 1.70158;
                        double p = 0;
                        float a = c;
                        if (t == 0) return b;
                        if ((t /= d) == 1) return b + c;
                        if (p == 0) p = d * .3;
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p)) + c + b;
                    }
                case mUIEasingType.easeInOutElastic:
                    {
                        double s = 1.70158;
                        double p = 0;
                        float a = c;
                        if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (p == 0) p = d * (.3 * 1.5);
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        if (t < 1) return (float)(-.5 * (a * Mathf.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
                        return (float)(a * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p)) * .5 + c + b);
                    }
                case mUIEasingType.easeInBack:
                    {
                        double s = 1.70158f;
                        return (float)(c * (t /= d) * t * ((s + 1) * t - s) + b);
                    }
                case mUIEasingType.easeOutBack:
                    {
                        double s = 1.70158f;
                        return (float)(c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b);
                    }
                case mUIEasingType.easeInOutBack:
                    {
                        double s = 1.70158f;
                        if ((t /= d / 2) < 1) return (float)(c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b);
                        return (float)(c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b);
                    }
                case mUIEasingType.easeInBounce:
                    return easeInBounce(c, t, d, b);
                case mUIEasingType.easeOutBounce:
                    return easeOutBounce(c, t, d, b);
                case mUIEasingType.easeInOutBounce:
                    if (t < d / 2) return (float)(easeInBounce(c, t, d, b) * .5 + b);
                    return (float)(easeOutBounce(c, t, d, b) * .5 + c * .5 + b);
                case mUIEasingType.linear:
                    return c * (t / d) + b;
                default:
                    return c * (t / d) + b;
            }
        }

        private static float easeInBounce(float c, float t, float d, float b = 0)
        {
            return c - easeOutBounce(c, t, d, b) + b;
        }

        private static float easeOutBounce(float c, float t, float d, float b = 0)
        {
            if ((t /= d) < (1 / 2.75))
                return (float)(c * (7.5625 * t * t) + b);
            if (t < (2 / 2.75))
                return (float)(c * (7.5625 * (t -= (1.5f / 2.75f)) * t + .75) + b);
            if (t < (2.5 / 2.75))
                return (float)(c * (7.5625 * (t -= (2.25f / 2.75f)) * t + .9375) + b);
            return (float)(c * (7.5625 * (t -= (2.625f / 2.75f)) * t + .984375) + b);
        }
    }
}
