using System;
using UnityEngine;

namespace mUIApp.Components
{
    [Flags]
    public enum UIVector3Set
    {
        SET_X = 0x0,
        SET_Y = 0x1,
        SET_Z = 0x2,
    }

    public static partial class UIElementsHelper
    {
        /*public static T SetParent<T>(this T obj, UIObject parent) where T : UIObject
        {
            return obj.SetParent(parent.Transform);
        }*/

        public static T SetParent<T>(this T obj, UIObject parent) where T : UIObject
        {
            obj.ChangeParent(parent);
            return obj;
        }

        public static Vector2 GetRelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIObject
        {
            return new Vector3(
                obj.Parent.Left + obj.Parent.Width * xRelative,
                obj.Parent.Bottom + obj.Parent.Height * yRelative,
                obj.Transform.position.z
            );
        }

        public static T RelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIObject
        {
            obj.Position(
                obj.Parent.Left + obj.Parent.Width * xRelative,
                obj.Parent.Bottom + obj.Parent.Height * yRelative,
                obj.Transform.position.z
            );
            return obj;
        }

        public static T RelativeTranslate<T>(this T obj, float xRelative, float yRelative = 0, Space space = Space.World) where T : UIObject
        {
            obj.Translate(
                obj.Parent.Width * xRelative,
                obj.Parent.Height * yRelative,
                0,
                space
            );
            return obj;
        }

        public static T Translate<T>(this T obj, float x, float y = 0, float z = 0, Space space = Space.World) where T : UIObject
        {
            obj.Translate(x, y, z, space);
            return obj;
        }

        public static Vector2 GetScale<T>(this T obj) where T : UIObject
        {
            return obj.Transform.localScale;
        }

        public static Vector3 GetRotation<T>(this T obj) where T : UIObject
        {
            return obj.Transform.eulerAngles;
        }

        public static Vector2 GetPos<T>(this T obj) where T : UIObject
        {
            return obj.Transform.position;
        }

        public static T Position<T>(this T obj, float x, float y) where T : UIObject
        {
            obj.Position(x, y, obj.Transform.position.z);
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other, UIVector3Set set) where T : UIObject
        {
            Vector3 newPos = obj.Transform.position;
            if ((set & UIVector3Set.SET_X) == UIVector3Set.SET_X)
                newPos.x = other.Transform.position.x;
            if ((set & UIVector3Set.SET_Y) == UIVector3Set.SET_Y)
                newPos.y = other.Transform.position.y;
            if ((set & UIVector3Set.SET_Z) == UIVector3Set.SET_Z)
                newPos.z = other.Transform.position.z;
            obj.Position(newPos.x, newPos.y, newPos.z);
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other) where T : UIObject
        {
            obj.Position(other.Transform.position.x, other.Transform.position.y, other.Transform.position.z);
            return obj;
        }

        public static T SortingOrder<T>(this T obj, int sortingOrder) where T : UIObject
        {
            obj.Renderer.sortingOrder = obj.Parent.Renderer.sortingOrder + sortingOrder;
            return obj;
        }

        public static T SetName<T>(this T obj, string name) where T : UIObject
        {
            obj.GameObject.name = name;
            return obj;
        }

        public static T Rotate<T>(this T obj, float x, float y, float z) where T : UIObject
        {
            obj.Rotate(x, y, z);
            return obj;
        }

        public static T Rotate<T>(this T obj, float z) where T : UIObject
        {
            obj.Rotate(obj.Transform.eulerAngles.x, obj.Transform.eulerAngles.y, z);
            return obj;
        }

        public static T Scale<T>(this T obj, float x, float y) where T : UIObject
        {
            obj.Scale(x, y, obj.Transform.eulerAngles.z);
            return obj;
        }

        public static T Scale<T>(this T obj, Vector3 scale) where T : UIObject
        {
            obj.Scale(scale.x, scale.y, scale.z);
            return obj;
        }

        public static T Active<T>(this T obj, bool active) where T : UIObject
        {
            obj.Active = active;
            return obj;
        }
    }
}
