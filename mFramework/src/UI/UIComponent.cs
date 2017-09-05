using System;
using System.Linq.Expressions;
using System.Reflection;

namespace mFramework.UI
{
    public abstract class UIComponentSettings
    {
        
    }

    public static class NewComponent<T> where T : UIComponent
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
    }

    public abstract class UIComponent : UIObject
    {
        protected UIComponent(UIObject parent) : base(parent)
        {
            SetName("UIComponent");
        }
        
        internal static T Create<T>(UIObject parent, UIComponentSettings settings = null) where T : UIComponent
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var component = NewComponent<T>.Instance(parent);

            //var component = (T)
            //    Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null,
            //        new object[] {parent}, CultureInfo.InvariantCulture);

            component.ApplySettings(settings);
            component.SetName(typeof(T).Name);
            parent.AddChildObject(component);

            return component;
        }

        protected virtual void ApplySettings(UIComponentSettings settings)
        {
        }
    }
}
