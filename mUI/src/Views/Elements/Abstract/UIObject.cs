using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UIObjectHelper
    {
        public static T SortingOrder<T>(this T obj, int sortingOrder) where T : UIObject
        {
            obj.Renderer.sortingOrder = sortingOrder;
            return obj;
        }

        public static T SetName<T>(this T obj, string name) where T : UIObject
        {
            obj.GameObject.name = name;
            return obj;
        }

        public static T Scale<T>(this T obj, float x, float y) where T : UIObject
        {
            Scale(obj.Transform, x, y);
            return obj;
        }

        public static T Scale<T>(this T obj, Vector3 scale) where T : UIObject
        {
            Scale(obj.Transform, scale.x, scale.y, scale.z);
            return obj;
        }

        public static void Scale(Transform transform, float x, float y)
        {
            transform.localScale = new Vector3(x, y, transform.localScale.z);
        }

        public static void Scale(Transform transform, float x, float y, float z)
        {
            transform.localScale = new Vector3(x, y, z);
        }
    }

    public abstract class UIObject
    {
        public abstract float Width { get; }
        public abstract float Height { get; }

        public int SortingOrder { get { return Renderer.sortingOrder; } }
        public SpriteRenderer Renderer { get; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }

        //private BaseView ParentView { get; }
        
        protected UIObject(BaseView view)
        {
            view.AddChildObject(this);

            GameObject = new GameObject("ui object");
            Transform = GameObject.transform;
            Transform.parent = view.Transform;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
        }
    }
}
