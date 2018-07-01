using System;

namespace mFramework.UI
{
    public interface IView : IUIObject
    {
        ushort? StencilId { get; }

        UIView View(Type viewType, UIViewProps props, params object[] @params);
        UIView View(Type viewType, params object[] @params);
        T View<T>(params object[] @params) where T : UIView;
        T View<T>(UIViewProps props, params object[] @params) where T : UIView;
    }
}