using UnityEngine;

namespace mFramework.UI
{
    public struct UIRect
    {
        public float Top;
        public float Bottom;
        public float Right;
        public float Left;
        public Vector2 Position;

        public override string ToString()
        {
            return $"Pos={Position} Top={Top} Bottom={Bottom} Right={Right} Left={Left}";
        }
    }
}
