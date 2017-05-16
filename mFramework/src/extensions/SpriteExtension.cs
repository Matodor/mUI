using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public static class SpriteExtension
    {
        public static Vector2 GetCenterOffset(this Sprite sprite)
        {
            var pivot = sprite.GetPivot();
            return new Vector2()
            {
                x = (-pivot.x + 0.5f) * sprite.bounds.size.x,
                y = (-pivot.y + 0.5f) * sprite.bounds.size.y
            };
        }
    

        public static Vector2 GetPivot(this Sprite sprite)
        {
            return new Vector2()
            {
                x = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f,
                y = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f
            };
        }
    }
}
