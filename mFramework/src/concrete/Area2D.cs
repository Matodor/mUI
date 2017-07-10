using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public enum AreaType
    {
        RECTANGLE = 0,    
    }

    public abstract class Area2D
    {
        public Vector2 Offset { get; set; } = Vector2.zero;
        public Vector2 Center { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.one;
        public float AdditionalScale = 1;

        public event Action<Area2D> Update;

        public virtual bool InArea(Vector2 worldPoint)
        {
            Update?.Invoke(this);
            return false;
        }

        public virtual void Draw() { }
    }
}
