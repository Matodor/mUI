using System;

namespace mFramework.UI
{
    public interface IView : IUIObject
    {
        ushort? StencilId { get; }

        UIView View(Type viewType, UIViewSettings settings, params object[] @params);
        UIView View(Type viewType, params object[] @params);
        T View<T>(params object[] @params) where T : UIView;
        T View<T>(UIViewSettings settings, params object[] @params) where T : UIView;
    }
}