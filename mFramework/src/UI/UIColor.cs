using System;
using System.Threading;
using UnityEngine;

namespace mFramework.UI
{
    public enum UIColorType
    {
        RGBA = 0,
        HSVA,
    }

    public class UIColor
    {
        public Color32 Color32 => ToColor32();

        public UIColorType Type { get; }
        public float n1; // r, h
        public float n2; // g, s
        public float n3; // b, v
        public float Alpha; // alpha

        public UIColor(string hexColor)
        {
            Type = UIColorType.RGBA;
            Color color;

            if (ColorUtility.TryParseHtmlString(hexColor, out color))
            {
                n1 = ((Color32)color).r;
                n2 = ((Color32)color).g;
                n3 = ((Color32)color).b;
                Alpha = ((Color32)color).a;
            }
            else
            {
                throw new Exception("Can't parse hex color");
            }
        }

        public UIColor(string hexColor, byte alpha)
        {
            Type = UIColorType.RGBA;
            Color color;

            if (ColorUtility.TryParseHtmlString(hexColor, out color))
            {
                n1 = ((Color32) color).r;
                n2 = ((Color32)color).g;
                n3 = ((Color32)color).b;
                Alpha = alpha;
            }
            else
            {
                throw new Exception("Can't parse hex color");
            }
        }

        public UIColor(Color32 color, UIColorType t = UIColorType.RGBA)
        {
            n1 = color.r;
            n2 = color.g;
            n3 = color.b;
            Alpha = color.a;
            Type = t;
        }

        public UIColor(float a1, float a2, float a3, float a4 = 255, UIColorType t = UIColorType.RGBA)
        {
            n1 = mMath.Clamp(a1, 0f, t == UIColorType.HSVA ? 359f : 255f);
            n2 = mMath.Clamp(a2, 0f, 255f);
            n3 = mMath.Clamp(a3, 0f, 255f);
            Alpha = mMath.Clamp(a4, 0f, 255f);
            Type = t;
        }
        
        private Color32 ToColor32()
        {
            if (Type == UIColorType.RGBA)
                return new Color32((byte)n1, (byte)n2, (byte)n3, (byte)Alpha);
            return HSVToRGB(n1, n2, n3, Alpha);
        }

        public override string ToString()
        {
            return $"Type: {Type} | n1: {n1} | n2: {n2} | n3: {n3} | Alpha: {Alpha}";
        }

        public UIColor Copy()
        {
            return new UIColor(n1, n2, n3, Alpha, Type);
        }

        public static Color FromHtmlString(string hexColor)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexColor, out color))
                return color;
            return Color.white;
        }

        public static Color HSVToRGB(float H, float S, float V, float A)
        {
            Color c;

            S /= 255f;
            V /= 255f;
            A /= 255f;

            if (H <= 180)
            {
                if (H <= 60) c = new Color(1, H / 60, 0);
                else if (H <= 120) c = new Color(1 - (H - 60) / 60, 1, 0);
                else c = new Color(0, 1, (H - 120) / 60);
            }
            else
            {
                if (H <= 240) c = new Color(0, 1 - (H - 180) / 60, 1);
                else if (H <= 300) c = new Color((H - 240) / 60, 0, 1);
                else c = new Color(1, 0, 1 - (H - 300) / 60);
            }

            c.r *= V;
            c.g *= V;
            c.b *= V;

            c.r += (V - c.r) * (1 - S);
            c.g += (V - c.g) * (1 - S);
            c.b += (V - c.b) * (1 - S);
            c.a = A;
            return c;
        }
    }
}
