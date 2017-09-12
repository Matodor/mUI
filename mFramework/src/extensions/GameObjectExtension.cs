using System;
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
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            if (transform == parent)
                throw new ArgumentNullException(nameof(parent));

            transform.parent = parent;
            transform.localPosition = Vector3.zero;
        }
    }
}
