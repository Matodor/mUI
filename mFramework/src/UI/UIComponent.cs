using System;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIComponentProps : UIObjectProps
    {

    }

    /*public static class NewComponent<T> where T : UIComponent
    {
        public static readonly Func<UIObject, T> Instance;

        static NewComponent()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] {typeof(UIObject)}, null);
            var parameter = Expression.Parameter(typeof(UIObject), "parent");
            var e = Expression.New(constructor, parameter);
            Instance = Expression.Lambda<Func<UIObject, T>>(e, parameter).Compile();
        } 
    }*/

    public abstract class UIComponent : UIObject
    {
        internal static T Create<T>(UIObject parent, UIComponentProps props = null) 
            where T : UIComponent
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (props == null)
                throw new Exception("UIComponent: UIComponentProps can not be null");

            var component = new GameObject(typeof(T).Name).AddComponent<T>();
            component.SetupParent(parent);
            component.ApplyProps(props);
            component.InitCompleted(true);
            mUI.ObjectCreated(component);
            return component;
        }

        protected virtual void ApplyProps(UIComponentProps props)
        {
            base.ApplyProps(props);
        }
    }
}
