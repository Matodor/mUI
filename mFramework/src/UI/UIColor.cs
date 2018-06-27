using UnityEngine;

namespace mFramework.UI
{
    public static class ColorExtensions
    {
        public static UIColor UIColor(this Color color)
        {
            return (UIColor) color;
        }

        public static UIColor UIColor(this Color color, byte opacity)
        {
            return new UIColor(color.r, color.g, color.b, opacity / 255f, 
                UI.UIColor.Type.RGBA);
        }

        public static Color Opacity(this Color color, byte opacity)
        {
            color.a = opacity / 255f;
            return color;
        }

        public static Color Opacity(this Color color, float opacity)
        {
            color.a = opacity;
            return color;
        }

        public static Color Grayscale(this Color color)
        {
            var y = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
            color.r = y;
            color.g = y;
            color.b = y;
            return color;
        }

        public static Color Inverted(this Color color)
        {
            return (Color) color.UIColor().ToHSV.Inverted();
        }

        public static UIColor Inverted(this UIColor color)
        {
            if (color.ColorType == UI.UIColor.Type.HSV)
            {
                return new UIColor(((color.N1 + 180) % 360) / 360f, color.N2, color.N3, color.Alpha,
                    UI.UIColor.Type.HSV);
            }

            return color.ToHSV.Inverted().ToRGBA;
        }

        /// <summary>
        /// Make color darken
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="percents">0% to 100%</param>
        /// <returns></returns>
        public static Color Darken(this Color color, int percents)
        {
            return color.ChangeColorBrightness(-percents / 100f);
        }

        /// <summary>
        /// Make color lighter
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="percents">0% to 100%</param>
        /// <returns></returns>
        public static Color Lighter(this Color color, int percents)
        {
            return color.ChangeColorBrightness(+percents / 100f);
        }

        /// <summary>
        /// Change color brightness
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="correctionFactor">[-1, 1]</param>
        /// <returns></returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                color.r *= correctionFactor;
                color.g *= correctionFactor;
                color.b *= correctionFactor;
            }
            else
            {
                color.r = (1f - color.r) * correctionFactor + color.r;
                color.g = (1f - color.g) * correctionFactor + color.g;
                color.b = (1f - color.b) * correctionFactor + color.b;
            }

            return color;
        }
    }

    public struct UIColor
    {
        public enum Type
        {
            RGBA = 0,
            HSV,
        }

        public UIColor ToRGBA => HSVToRGBA(this);
        public UIColor ToHSV => RGBAToHSV(this);
        public UIColor ToGrayscale => RGBAToGrayscale(this);

        /// <summary>
        /// Color type RGBA or HSV
        /// </summary>
        public readonly Type ColorType;

        /// <summary>
        /// Normalized color component (R - rgba, H - hsv)
        /// </summary>
        public float N1;

        /// <summary>
        /// Normalized color component (G - rgba, S - hsv)
        /// </summary>
        public float N2;

        /// <summary>
        /// Normalized color component (B - rgba, V - hsv)
        /// </summary>
        public float N3;

        /// <summary>
        /// Normalized opacity (alpha channel)
        /// </summary>
        public float Alpha;

        public UIColor(string hexColor)
        {
            this = FromHTML(hexColor);
        }

        public UIColor Opacity(float opacity)
        {
            var color = this;
            color.Alpha = opacity;
            return color;
        }

        public UIColor Opacity(byte opacity)
        {
            var color = this;
            color.Alpha = opacity / 255f;
            return color;
        }

        public UIColor(float n1, float n2, float n3, float alpha, Type type)
        {
            N1 = n1;
            N2 = n2;
            N3 = n3;
            Alpha = alpha;
            ColorType = type;
        }

        /// <summary>
        /// Create UIColor by the given color in html format
        /// </summary>
        /// <param name="hexColor">Html color in hex format (example "#ffffff")</param>
        /// <param name="opacity">Normilized opacity</param>
        /// <returns></returns>
        public static UIColor FromHTML(string hexColor, float opacity)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
                return new UIColor(1, 1, 1, 1, Type.RGBA);

            color.a = opacity;
            return (UIColor)color;
        }

        /// <summary>
        /// Create UIColor by the given color in html format
        /// </summary>
        /// <param name="hexColor">Html color in hex format (example "#ffffff")</param>
        /// <param name="opacity">Opacity</param>
        /// <returns></returns>
        public static UIColor FromHTML(string hexColor, byte opacity)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
                return new UIColor(1, 1, 1, 1, Type.RGBA);

            color.a = opacity / 255f;
            return (UIColor) color;
        }

        /// <summary>
        /// Create UIColor by the given color in html format
        /// </summary>
        /// <param name="hexColor">Html color in hex format (example "#ffffff")</param>
        /// <returns></returns>
        public static UIColor FromHTML(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
                return (UIColor) color;
            return new UIColor(1, 1, 1, 1, Type.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given normalized r,g,b,a color components
        /// </summary>
        /// <param name="r">Normalized red color component</param>
        /// <param name="g">Normalized green color component</param>
        /// <param name="b">Normalized blue color component</param>
        /// <param name="a">Normalized alpha color component</param>
        /// <returns></returns>
        public static UIColor RGBA(float r, float g, float b, float a = 1)
        {
            return new UIColor(r, g, b, a, Type.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="r">Red color component 0-255</param>
        /// <param name="g">Green color component 0-255</param>
        /// <param name="b">Blue color component 0-255</param>
        /// <param name="a">Alpha color component 0-255</param>
        /// <returns></returns>
        public static UIColor RGBA(byte r, byte g, byte b, byte a = 255)
        {
            return new UIColor(r / 255f, g / 255f, b / 255f, a / 255f, Type.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="h">Normalized hue color component</param>
        /// <param name="s">Normalized saturation  color component</param>
        /// <param name="v">Normalized value color component</param>
        /// <param name="a">Normalized alpha color component</param>
        /// <returns></returns>
        public static UIColor HSV(float h, float s, float v, float a = 1)
        {
            return new UIColor(h, s, v, a, Type.HSV);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="h">Hue color component 0-360</param>
        /// <param name="s">Saturation  color component 0-255</param>
        /// <param name="v">Value color component 0-255</param>
        /// <param name="a">Alpha color component 0-255</param>
        /// <returns></returns>
        public static UIColor HSV(int h, byte s, byte v, byte a = 255)
        {
            return new UIColor(h / 360f, s / 255f, v / 255f, a / 255f, Type.HSV);
        }

        public static UIColor RGBAToHSV(UIColor color)
        {
            if (color.ColorType == Type.HSV)
                return color;

            Color.RGBToHSV((Color) color, out var h, out var s, out var v);
            return new UIColor(h, s, v, color.Alpha, Type.HSV);
        }

        public static UIColor HSVToRGBA(UIColor color)
        {
            if (color.ColorType == Type.RGBA)
                return color;

            return (UIColor) Color.HSVToRGB(color.N1, color.N2, color.N3);
        }

        public static UIColor RGBAToGrayscale(UIColor color)
        {
            color = color.ColorType == Type.RGBA ? color : HSVToRGBA(color);
            var y = 0.2126f * color.N1 + 0.7152f * color.N2 + 0.0722f * color.N3;
            return new UIColor(y, y, y, color.Alpha, Type.RGBA);
        }

        public static explicit operator Color(UIColor color)
        {
            color = color.ColorType == Type.RGBA ? color : HSVToRGBA(color);
            return new Color(color.N1, color.N2, color.N3, color.Alpha);
        }

        public static explicit operator UIColor(Color color)
        {
            return new UIColor(color.r, color.g, color.b, color.a, Type.RGBA);
        }

        public static UIColor Lerp(UIColor a, UIColor b, float t)
        {
            t = Mathf.Clamp01(t);
            return new UIColor(
                a.N1 + (b.N1 - a.N1) * t,
                a.N2 + (b.N2 - a.N2) * t,
                a.N3 + (b.N3 - a.N3) * t,
                a.Alpha + (b.Alpha - a.Alpha) * t,
                a.ColorType
            );
        }
    }
}