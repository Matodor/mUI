using UnityEngine;

namespace mFramework.UI
{
    public enum UIRectType
    {
        UNSCALED,
        LOCAL,
        GLOBAL
    }

    public struct UIRect
    {
        public UIRectType Type { get; set; }
        public Vector2 UpperLeft { get; set; }
        public Vector2 UpperCenter { get; set; }
        public Vector2 UpperRight { get; set; }

        public Vector2 LowerLeft { get; set; }
        public Vector2 LowerCenter { get; set; }
        public Vector2 LowerRight { get; set; }

        public Vector2 MiddleLeft { get; set; }
        public Vector2 MiddleCenter { get; set; }
        public Vector2 MiddleRight { get; set; }
    }
}
