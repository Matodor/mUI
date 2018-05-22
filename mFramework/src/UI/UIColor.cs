using UnityEngine;

namespace mFramework.UI
{
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
            return new UIColor(255f / r, 255f / g, 255f / b, 255f / a, Type.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="h">Normalized hue color component 0-360</param>
        /// <param name="s">Normalized saturation  color component 0-255</param>
        /// <param name="v">Normalized value color component 0-255</param>
        /// <param name="a">Normalized alpha color component 0-255</param>
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
            return new UIColor(360f / h, 255f / s, 255f / v, 255f / a, Type.HSV);
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