using System;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static class ViewExtensions
    {
        public static UIView ChildView(this IView view, Type viewType, params object[] @params)
        {
            return ChildView(view, viewType, new UIViewSettings
            {
                Width = view.GetWidth(),
                Height = view.GetHeight()
            }, @params);
        }

        public static UIView ChildView(this IView view, Type viewType, UIViewSettings settings, params object[] @params)
        {
            return UIView.Create(viewType, settings, view, @params);
        }

        public static T ChildView<T>(this IView view, params object[] @params) where T : UIView
        {
            return ChildView<T>(view, new UIViewSettings
            {
                Width = view.GetWidth(),
                Height = view.GetHeight()
            }, @params);
        }

        public static T ChildView<T>(this IView view, UIViewSettings settings, params object[] @params) where T : UIView
        {
            return UIView.Create<T>(settings, view, @params);
        }

        public static float RelativeX(this IView view, float t)
        {
            var rect = view.GetRect();
            return BezierHelper.Linear(t, rect.Left, rect.Right);
        }

        public static float RelativeY(this IView view, float t)
        {
            var rect = view.GetRect();
            return BezierHelper.Linear(t, rect.Bottom, rect.Top);
        }

        public static Vector2 Relative(this IView view, float tX, float yY)
        {
            var rect = view.GetRect();
            return new Vector2(
                BezierHelper.Linear(tX, rect.Left, rect.Right),
                BezierHelper.Linear(tX, rect.Bottom, rect.Top)
            );
        }
    }
}