using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Animations;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    [Flags]
    public enum UIVector3Set
    {
        SET_X = 0x0,
        SET_Y = 0x1,
        SET_Z = 0x2,
    }

    public static class UIObjectHelper
    {
        public static Vector2 GetRelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIGameObject
        {
            return  new Vector3(
                obj.ParentView.LeftAnchor + obj.ParentView.Width * xRelative,
                obj.ParentView.BottomAnchor + obj.ParentView.Height * yRelative,
                obj.Transform.position.z
            );
        }

        public static T RelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIGameObject
        {
            obj.Transform.position = new Vector3(
                obj.ParentView.LeftAnchor + obj.ParentView.Width*xRelative,
                obj.ParentView.BottomAnchor + obj.ParentView.Height*yRelative,
                obj.Transform.position.z
            );
            return obj;
        }

        public static T RelativeTranslate<T>(this T obj, float xRelative, float yRelative = 0, Space space = Space.World) where T : UIGameObject
        {
            obj.Transform.Translate(
                obj.ParentView.Width*xRelative,
                obj.ParentView.Height*yRelative,
                0,
                space
            );
            return obj;
        }

        public static T Translate<T>(this T obj, float x, float y = 0, Space space = Space.World) where T : UIGameObject
        {
            obj.Transform.Translate(x, y, 0, space);
            return obj;
        }

        public static Vector2 Position<T>(this T obj) where T : UIGameObject
        {
            return obj.Transform.position;
        }

        public static T Position<T>(this T obj, float x, float y) where T : UIGameObject
        {
            obj.Transform.position = new Vector3(x, y, obj.Transform.position.z);
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other, UIVector3Set set) where T : UIGameObject
        {
            Vector3 newPos = obj.Transform.position;
            if ((set & UIVector3Set.SET_X) == UIVector3Set.SET_X)
                newPos.x = other.Transform.position.x;
            if ((set & UIVector3Set.SET_Y) == UIVector3Set.SET_Y)
                newPos.y = other.Transform.position.y;
            if ((set & UIVector3Set.SET_Z) == UIVector3Set.SET_Z)
                newPos.z = other.Transform.position.z;
            obj.Transform.position = newPos;
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other) where T : UIGameObject
        {
            obj.Transform.position = other.Transform.position;
            return obj;
        }

        public static T SortingOrder<T>(this T obj, int sortingOrder) where T : UIObject
        {
            obj.Renderer.sortingOrder = obj.ParentView.SortingOrder + sortingOrder;
            return obj;
        }

        public static T SetName<T>(this T obj, string name) where T : UIGameObject
        {
            obj.GameObject.name = name;
            return obj;
        }

        public static T Scale<T>(this T obj, float x, float y) where T : UIGameObject
        {
            Scale(obj.Transform, x, y);
            return obj;
        }

        public static T Scale<T>(this T obj, Vector3 scale) where T : UIGameObject
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

    public abstract class UIObject : UIGameObject
    {
        public abstract float Width { get; }
        public abstract float Height { get; }

        public SpriteRenderer Renderer { get; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public BaseView ParentView { get; }
        public List<mUIAnimation> Animations { get; } = new List<mUIAnimation>();

        protected UIObject(BaseView view)
        {
            ParentView = view;
            GameObject = new GameObject("UIObject");
            Transform = GameObject.transform;
            Transform.parent = view.Transform;
            Transform.localPosition = Vector3.zero;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
            Renderer.sortingOrder = view.SortingOrder;

            view.AddChildObject(this);
        }

        public void Tick()
        {
            for (int i = Animations.Count - 1; i >= 0; i--)
                Animations[i].Tick();
            OnTick();   
        }

        protected virtual void OnTick() { }
    }
}
