using System;
using System.Collections.Generic;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static T Anchor<T>(this T obj, UIAnchor newAnchor) where T : IUIObject
        {
            obj.Anchor = newAnchor;
            return obj;
        }

        public static T SortingOrder<T>(this T obj, int localOrder) where T : IUIObject
        {
            obj.LocalSortingOrder = localOrder;
            return obj;
        }

        public static Vector3 TranslatedPos<T>(this T obj, float shiftX, float shiftY,
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                return (obj as UIObject).Transform.localPosition + new Vector3(shiftX, shiftY, 0);
            
            // ReSharper disable once PossibleNullReferenceException
            return (obj as UIObject).Transform.position + new Vector3(shiftX, shiftY, 0);
        }

        public static Vector3 TranslatedPos<T>(this T obj, Vector2 shift,
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                return (obj as UIObject).Transform.localPosition + new Vector3(shift.x, shift.y);
            
            // ReSharper disable once PossibleNullReferenceException
            return (obj as UIObject).Transform.position + new Vector3(shift.x, shift.y);
        }

        public static Vector3 TranslatedPos<T>(this T obj, Vector3 shift,
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                return (obj as UIObject).Transform.localPosition + shift;

            // ReSharper disable once PossibleNullReferenceException
            return (obj as UIObject).Transform.position + shift;
        }

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

        public static T Translate<T>(this T obj, float shiftX, float shiftY, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.localPosition += new Vector3(shiftX, shiftY, 0);
            else
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.position += new Vector3(shiftX, shiftY, 0);
            return obj;
        }

        public static T Translate<T>(this T obj, Vector2 shift, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.localPosition += new Vector3(shift.x, shift.y);
            else
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.position += new Vector3(shift.x, shift.y);
            return obj;
        }

        public static T Translate<T>(this T obj, Vector3 shift, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.localPosition += shift;
            else
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).Transform.position += shift;
            return obj;
        }

        public static T Position<T>(this T obj, float x, float y, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                obj.LocalPosition = new Vector2(x, y);
            else
                obj.Position = new Vector2(x, y);
            return obj;
        }

        public static T Position<T>(this T obj, Vector2 position, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                obj.LocalPosition = position;
            else
                obj.Position = position;
            return obj;
        }

        public static T Position<T>(this T obj, Vector3 position, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                obj.LocalPosition = position;
            else
                obj.Position = position;
            return obj;
        }

        public static T Position<T>(this T obj, Vector2 position,
            UIAnchor anchor, Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).LocalPositionByAnchor(position, anchor);
            else
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).PositionByAnchor(position, anchor);
            return obj;
        }

        public static T Position<T>(this T obj, Vector3 position, UIAnchor anchor,
            Space relativeTo = Space.World) where T : IUIObject
        {
            if (relativeTo == Space.Self)
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).LocalPositionByAnchor(position, anchor);
            else
                // ReSharper disable once PossibleNullReferenceException
                (obj as UIObject).PositionByAnchor(position, anchor);
            return obj;
        }

        public static Vector3 GetPos<T>(this T obj, UIAnchor anchor, 
            Space relativeTo = Space.World) where T : IUIObject
        {
            return relativeTo == Space.Self
                // ReSharper disable once PossibleNullReferenceException
                ? (obj as UIObject).GetLocalAnchorPos(anchor)
                // ReSharper disable once PossibleNullReferenceException
                : (obj as UIObject).GetAnchorPos(anchor);
        }

        public static T Color<T>(this T obj, Color color) where T : IUIColored
        {
            obj.Color = color;
            return obj;
        }

        public static T Color<T>(this T obj, UIColor color) where T : IUIColored
        {
            obj.Color = (Color) color;
            return obj;
        }

        /// <summary>
        /// Set not normilized opacity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">IUIColored object</param>
        /// <param name="opacity">Opacity (0 - 255)</param>
        /// <returns></returns>
        public static T Opacity<T>(this T obj, float opacity) where T : IUIColored
        {
            obj.Opacity = mMath.Clamp(opacity, 0f, 255f) / 255f;
            return obj;
        }

        public static void Color(this IEnumerable<IUIColored> objs, Color color)
        {
            foreach (var o in objs)
                o.Color = color;
        }

        public static void Color(this IEnumerable<IUIColored> objs, UIColor color)
        {
            var ucolor = (Color) color;
            foreach (var o in objs)
                o.Color = ucolor;
        }

        /// <summary>
        /// Set not normilized opacity for each item
        /// </summary>
        /// <param name="objs">Collection of IUIColored objects</param>
        /// <param name="opacity">Opacity (0 - 255)</param>
        public static void Opacity(this IEnumerable<IUIColored> objs, float opacity)
        {
            opacity = mMath.Clamp(opacity, 0f, 255f) / 255f;
            foreach (var o in objs)
                o.Opacity = opacity;
        }

        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action) 
            where T : IUIObject
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