using UnityEngine;

namespace mFramework
{
    public static class VectorExtension
    {
        public static float Length(this Vector2 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }
    }
}