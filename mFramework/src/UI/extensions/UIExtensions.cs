using System;
using System.Collections.Generic;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static T Rotation<T>(this T obj, float value) where T : IUIObject
        {
            obj.Rotation = value;
            return obj;
        }

        public static T Scale<T>(this T obj, float x, float y) where T : IUIObject
        {
            obj.Scale = new Vector2(x, y);
            return obj;
        }

        public static T Scale<T>(this T obj, Vector2 scale) where T : IUIObject
        {
            obj.Scale = scale;
            return obj;
        }

        public static T Scale<T>(this T obj, Vector3 scale) where T : IUIObject
        {
            obj.Scale = scale;
            return obj;
        }

        public static T Position<T>(this T obj, float x, float y) where T : IUIObject
        {
            obj.Position = new Vector2(x, y);
            return obj;
        }

        public static T Position<T>(this T obj, Vector2 position) where T : IUIObject
        {
            obj.Position = position;
            return obj;
        }

        public static T Position<T>(this T obj, Vector3 position) where T : IUIObject
        {
            obj.Position = position;
            return obj;
        }

        public static void Disable(this IEnumerable<IUIClickable> objs)
        {
            foreach (var o in objs)
                o.UiClickableOld.Enabled = false;
        }

        public static void Enable(this IEnumerable<IUIClickable> objs)
        {
            foreach (var o in objs)
                o.UiClickableOld.Enabled = true;
        }

        public static void SetColor(this IEnumerable<IUIColored> objs, Color color)
        {
            foreach (var o in objs)
                o.SetColor(color);
        }

        public static void SetColor(this IEnumerable<IUIColored> objs, UIColor color)
        {
            foreach (var o in objs)
                o.SetColor(color);
        }

        public static void SetOpacity(this IEnumerable<IUIColored> objs, float opacity)
        {
            opacity = mMath.Clamp(opacity, 0f, 255f);
            foreach (var o in objs)
                o.SetOpacity(opacity);
        }

        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action) where T : IUIObject
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }
        
        public static IEnumerable<T> DeepChilds<T>(this IUIObject obj) where T : IUIObject
        {
            foreach (var child in obj.Childs)
            {
                foreach (var x1 in DeepChildsImpl<T>(child))
                    yield return x1;
            }
        }

        private static IEnumerable<T> DeepChildsImpl<T>(IUIObject obj) where T : IUIObject
        {
            if (obj is T ret1)
                yield return ret1;

            foreach (var child in obj.Childs)
            {
                foreach (var x1 in DeepChildsImpl<T>(child))
                    yield return x1;
            }
        }

        public static bool DeepFind(this IUIObject obj, Predicate<IUIObject> predicate, out IUIObject value)
        {
            if (predicate(obj))
            {
                value = obj;
                return true;
            }

            var tmp = obj.Childs.LastItem;
            while (tmp != null)
            {
                if (tmp.Value.DeepFind(predicate, out value))
                {
                    return true;
                }
                tmp = tmp.Prev;
            }

            value = null;
            return false;
        }
    }
}