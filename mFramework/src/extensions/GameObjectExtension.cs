using System;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static class GameObjectExtension
    {
        public static Transform SetParentTransform(this Transform transform, Transform parent)
        {
            SetParentTransformImpl(transform, parent);
            return transform;
        }

        public static GameObject SetParentTransform(this GameObject gameObject, GameObject parent)
        {
            SetParentTransformImpl(gameObject.transform, parent.transform);
            return gameObject;
        }

        public static GameObject SetParentTransform(this GameObject gameObject, UIObject parent)
        {
            SetParentTransformImpl(gameObject.transform, parent.transform);
            return gameObject;
        }

        private static void SetParentTransformImpl(Transform transform, Transform parent)
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
