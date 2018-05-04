using System;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static class TransformExtension
    {
        public static Transform ParentTransform(this Transform transform, Transform parent)
        {
            SetParentTransformImpl(transform, parent);
            return transform;
        }

        public static GameObject ParentTransform(this GameObject gameObject, GameObject parent)
        {
            SetParentTransformImpl(gameObject.transform, parent.transform);
            return gameObject;
        }

        public static GameObject ParentTransform(this GameObject gameObject, UIObject parent)
        {
            SetParentTransformImpl(gameObject.transform, parent.Transform);
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
