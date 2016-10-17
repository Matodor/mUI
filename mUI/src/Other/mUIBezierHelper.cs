using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Other
{
    public static class mUIBezierHelper
    {
        public static Vector2 Linear(float t, Vector2 p0, Vector2 p1)
        {
            return (1 - t)*p0 + t*p1;
        }

        public static Vector2 Quadratic(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (1 - t)*(1 - t)*p0 + 2*t*(1 - t)*p1 + t*t*p2;
        }

        public static Vector2 Cubic(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (1 - t)*(1 - t)*(1 - t)*p0 + 3*t*(1 - t)*(1 - t)*p1 + 3*t*t*(1 - t)*p2 + t*t*t*p3;
        }
    }
}
