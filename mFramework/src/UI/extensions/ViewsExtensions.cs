using System;
using mFramework.UI;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static UIView View(this IView view, Type viewType, params object[] @params)
        {
            return View(view, viewType, new UIViewSettings
            {
                Width = view.Width,
                Height = view.Height,
            }, @params);
        }

        public static UIView View(this IView view, Type viewType, UIViewSettings settings, params object[] @params)
        {
            return UIView.Create(viewType, settings, view, @params);
        }

        public static T View<T>(this IView view, params object[] @params) where T : UIView
        {
            return View<T>(view, new UIViewSettings
            {
                Width = view.Width,
                Height = view.Height,
            }, @params);
        }

        public static T View<T>(this IView view, UIViewSettings settings, params object[] @params) where T : UIView
        {
            return UIView.Create<T>(settings, view, @params);
        }
    }
}