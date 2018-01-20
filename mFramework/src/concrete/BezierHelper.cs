using UnityEngine;

namespace mFramework
{
    public static class BezierHelper
    {
        public static Color Linear(float t, Color c0, Color c1)
        {
            var r = Linear(t, c0.r, c1.r);
            var g = Linear(t, c0.g, c1.g);
            var b = Linear(t, c0.b, c1.b);
            var a = Linear(t, c0.a, c1.a);
            return new Color(r, g, b, a);
        }

        public static float Linear(float t, float p0, float p1)
        {
            return (1 - t) * p0 + t * p1;
        }

        public static Vector2 Linear(float t, Vector2 p0, Vector2 p1)
        {
            return (1 - t) * p0 + t * p1;
        }

        public static Color Quadratic(float t, Color c0, Color c1, Color c2)
        {
            var r = Quadratic(t, c0.r, c1.r, c2.r);
            var g = Quadratic(t, c0.g, c1.g, c2.g);
            var b = Quadratic(t, c0.b, c1.b, c2.b);
            var a = Quadratic(t, c0.a, c1.a, c2.a);
            return new Color(r, g, b, a);
        }

        public static float Quadratic(float t, float p0, float p1, float p2)
        {
            return (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p1 + t * t * p2;
        }

        public static Vector2 Quadratic(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p1 + t * t * p2;
        }

        public static Color Cubic(float t, Color c0, Color c1, Color c2, Color c3)
        {
            var r = Cubic(t, c0.r, c1.r, c2.r, c3.r);
            var g = Cubic(t, c0.g, c1.g, c2.g, c3.g);
            var b = Cubic(t, c0.b, c1.b, c2.b, c3.b);
            var a = Cubic(t, c0.a, c1.a, c2.a, c3.a);
            return new Color(r, g, b, a);
        }

        public static float Cubic(float t, float p0, float p1, float p2, float p3)
        {
            return (1 - t) * (1 - t) * (1 - t) * p0 + 3 * t * (1 - t) * (1 - t) * p1 + 3 * t * t * (1 - t) * p2 + t * t * t * p3;
        }

        public static Vector2 Cubic(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (1 - t) * (1 - t) * (1 - t) * p0 + 3 * t * (1 - t) * (1 - t) * p1 + 3 * t * t * (1 - t) * p2 + t * t * t * p3;
        }
    }
}
