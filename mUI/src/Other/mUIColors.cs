using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Other
{
    public enum UIColorType
    {
        RGBA = 0,
        HSVA,
    }

    public enum UIGradientType
    {
        TOP_TO_BOTTOM,
        BOTTOM_TO_TOP,
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT,
    }

    public class mUIGradient
    {
        public mUIColor LeftTop;
        public mUIColor LeftBottom;
        public mUIColor RightTop;
        public mUIColor RightBottom;

        public mUIGradient(mUIColor a, mUIColor b, mUIColor c, mUIColor d)
        {
            LeftBottom = a;
            LeftTop = b;
            RightTop = c;
            RightBottom = d;
        }

        public mUIGradient(mUIColor a, mUIColor b, UIGradientType type)
        {
            switch (type)
            {
                case UIGradientType.TOP_TO_BOTTOM:
                    LeftTop = a;
                    RightTop = a;
                    LeftBottom = b;
                    RightBottom = b;
                    break;
                case UIGradientType.BOTTOM_TO_TOP:
                    LeftTop = b;
                    RightTop = b;
                    LeftBottom = a;
                    RightBottom = a;
                    break;
                case UIGradientType.LEFT_TO_RIGHT:
                    LeftTop = a;
                    LeftBottom = a;
                    RightBottom = b;
                    RightTop = b;
                    break;
                case UIGradientType.RIGHT_TO_LEFT:
                    LeftTop = b;
                    LeftBottom = b;
                    RightBottom = a;
                    RightTop = a;
                    break;
                default:
                    LeftTop = a;
                    LeftBottom = a;
                    RightTop = b;
                    RightBottom = b;
                    break;
            }
        }
    }

    public class mUIColor
    {
        public UIColorType Type;
        public float n1; // r, h
        public float n2; // g, s
        public float n3; // b, v
        public float Aplha; // a, a

        public Color32 Color32
        {
            get
            {
                return mUIColorsHelper.ToColor32(this);
            }
        }

        public mUIColor FromAlpha(float alpha)
        {
            return new mUIColor(n1, n2, n3, alpha);
        }

        public mUIColor(float a1, float a2, float a3, float a4 = 255, UIColorType t = UIColorType.RGBA)
        {
            n1 = Mathf.Clamp(a1, 0, 255);
            n2 = Mathf.Clamp(a2, 0, 255);
            n3 = Mathf.Clamp(a3, 0, 255);
            Aplha = Mathf.Clamp(a4, 0, 255);
            Type = t;
        }
    }

    public static class mUIColors
    {
        public static mUIColor Black = new mUIColor(0, 0, 0);
        public static mUIColor White = new mUIColor(255, 255, 255);
    }

    public static class mUIColorsHelper
    {
        public static Color32 ToColor32(mUIColor inner)
        {
            if (inner.Type != UIColorType.RGBA)
                return HSVToRGB(inner.n1, inner.n2 / 255, inner.n3 / 255, inner.Aplha / 255);
            return new Color32((byte)inner.n1, (byte)inner.n2, (byte)inner.n3, (byte)inner.Aplha);
        }

        private static Color HSVToRGB(float H, float S, float V, float A)
        {
            Color c;
            c.a = A;

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
