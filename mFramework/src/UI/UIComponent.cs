using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIComponentSettings
    {
        
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

            var component = (T)
                Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new object[] {parent}, CultureInfo.InvariantCulture);

            component.ApplySettings(settings);
            component.SetName(typeof(T).Name);
            return component;
        }

        protected virtual void ApplySettings(UIComponentSettings settings)
        {
        }
    }
}
