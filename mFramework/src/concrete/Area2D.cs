﻿using System;
using UnityEngine;

namespace mFramework
{
    public enum AreaType
    {
        RECTANGLE = 0,    
    }

    public abstract class Area2D
    {
        public Vector2 Offset = Vector2.zero;
        public Vector2 Center;
        public float Rotation;
        public float AdditionalScale = 1;

        public event Action<Area2D> Update = _ => { };

        public abstract bool InArea(Vector2 worldPos);

        protected void UpdateData()
        {
            Update.Invoke(this);
        }

    }
}
