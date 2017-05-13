using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public static class GameObjectExtension
    {
        public static Transform SetParent(this Transform transform, Transform parent)
        {
            SetParentImpl(transform, parent);
            return transform;
        }

        public static GameObject SetParent(this GameObject gameObject, GameObject parent)
        {
            SetParentImpl(gameObject.transform, parent.transform);
            return gameObject;
        }

        private static void SetParentImpl(Transform transform, Transform parent)
        {
            if (transform == null || parent == null)
                throw new NullReferenceException("GameObjectExtension (SetParentImpl): the given transform was null");
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
        }
    }
}
