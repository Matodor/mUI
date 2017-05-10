using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public static class GameObjectExtension
    {
        public static IGameObject SetParent(this IGameObject instance, IGameObject parentInstance)
        {
            SetParentImpl(instance.GameObject, parentInstance.GameObject);
            return instance;
        }

        public static IGameObject SetParent(this IGameObject instance, GameObject parent)
        {
            SetParentImpl(instance.GameObject, parent);
            return instance;
        }

        public static GameObject SetParent(this GameObject gameObject, GameObject parent)
        {
            SetParentImpl(gameObject, parent);
            return gameObject;
        }

        private static void SetParentImpl(GameObject gameObject, GameObject parent)
        {
            if (gameObject == null || parent == null)
                throw new NullReferenceException("SetParentImpl");
            gameObject.transform.parent = parent.transform;
            gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
