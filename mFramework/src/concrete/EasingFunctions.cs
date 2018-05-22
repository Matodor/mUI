using System;
using UnityEngine;

namespace mFramework
{
    public enum EasingType
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

    public static class EasingFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easingType"></param>
        /// <param name="c">Current value</param>
        /// <param name="t">Current time [0..Duration]</param>
        /// <param name="d">Duration</param>
        /// <param name="b">Offset</param>
        /// <returns></returns>
        public static float GetValue(EasingType easingType, float c, float t, 
            float d, float b = 0)
        {
            switch (easingType)
            {
                case EasingType.easeInQuad:
                    return c * (t /= d) * t + b;

                case EasingType.easeOutQuad:
                    return -c * (t /= d) * (t - 2) + b;

                case EasingType.easeInOutQuad:
                    if ((t /= d / 2) < 1) return c / 2 * t * t + b;
                    return -c / 2 * ((--t) * (t - 2) - 1) + b;

                case EasingType.easeInCubic:
                    return c * (t /= d) * t * t + b;

                case EasingType.easeOutCubic:
                    return c * ((t = t / d - 1) * t * t + 1) + b;

                case EasingType.easeInOutCubic:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t + 2) + b;

                case EasingType.easeInQuart:
                    return c * (t /= d) * t * t * t + b;

                case EasingType.easeOutQuart:
                    return -c * ((t = t / d - 1) * t * t * t - 1) + b;

                case EasingType.easeInOutQuart:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
                    return -c / 2 * ((t -= 2) * t * t * t - 2) + b;

                case EasingType.easeInQuint:
                    return c * (t /= d) * t * t * t * t + b;

                case EasingType.easeOutQuint:
                    return c * ((t = t / d - 1) * t * t * t * t + 1) + b;

                case EasingType.easeInOutQuint:
                    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;

                case EasingType.easeInSine:
                    return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;

                case EasingType.easeOutSine:
                    return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;

                case EasingType.easeInOutSine:
                    return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;

                case EasingType.easeInExpo:
                    return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;

                case EasingType.easeOutExpo:
                    return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;

                case EasingType.easeInOutExpo:
                    if (t == 0) return b;
                    if (t == d) return b + c;
                    if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
                    return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;

                case EasingType.easeInCirc:
                    return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;

                case EasingType.easeInOutCirc:
                    if ((t /= d / 2) < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
                    return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;

                case EasingType.easeInElastic:
                    {
                        var s = 1.70158;
                        double p = 0;
                        var a = c;
                        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        return -(a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p))) + b;
                    }

                case EasingType.easeOutElastic:
                    {
                        var s = 1.70158;
                        double p = 0;
                        var a = c;
                        if (t == 0) return b;
                        if ((t /= d) == 1) return b + c;
                        if (p == 0) p = d * .3;
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p)) + c + b;
                    }

                case EasingType.easeInOutElastic:
                    {
                        double s;
                        double p = 0;
                        var a = c;
                        if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (p == 0) p = d * (.3 * 1.5);
                        if (a < Mathf.Abs(c)) { a = c; s = p / 4; }
                        else s = p / (2 * Math.PI);
                        if (t < 1) return (float)(-.5 * (a * Mathf.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
                        return (float)(a * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((float)((t * d - s) * (2 * Math.PI) / p)) * .5 + c + b);
                    }

                case EasingType.easeInBack:
                    {
                        double s = 1.70158f;
                        return (float)(c * (t /= d) * t * ((s + 1) * t - s) + b);
                    }

                case EasingType.easeOutBack:
                    {
                        double s = 1.70158f;
                        return (float)(c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b);
                    }

                case EasingType.easeInOutBack:
                    {
                        double s = 1.70158f;
                        if ((t /= d / 2) < 1) return (float)(c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b);
                        return (float)(c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b);
                    }

                case EasingType.easeInBounce:
                    return EaseInBounce(c, t, d, b);

                case EasingType.easeOutBounce:
                    return EaseOutBounce(c, t, d, b);

                case EasingType.easeInOutBounce:
                    return EaseInOutBounce(c, t, d, b);
                    
                case EasingType.linear:
                    return c * (t / d) + b;

                default:
                    return c * (t / d) + b;
            }
        }

        private static float EaseInBounce(float c, float t, float d, float b = 0)
        {
            return c - EaseOutBounce(c, d - t, d, b) + b;
        }

        private static float EaseOutBounce(float c, float t, float d, float b = 0)
        {
            if ((t /= d) < (1f / 2.75f))
                return c * (7.5625f * t * t) + b;
            if (t < (2f / 2.75f))
                return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
            if (t < (2.5 / 2.75))
                return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
            return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
        }

        private static float EaseInOutBounce(float c, float t, float d, float b = 0)
        {
            if (t < d / 2)
                return EaseInBounce(c, t * 2, d) * 0.5f + b;
            return EaseOutBounce(c, t * 2 - d, d) * 0.5f + c * 0.5f + b;
        }
    }
}
