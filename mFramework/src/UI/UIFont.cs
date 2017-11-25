using UnityEngine;

namespace mFramework.UI
{
    public class UIFont
    {
        public float Harshness { get; set; }
        public readonly Font Font;

        public UIFont(float harshness, Font font)
        {
            Font = font;
            Harshness = harshness;
        }
    }
}