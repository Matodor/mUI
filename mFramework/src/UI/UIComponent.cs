using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            _gameObject.name = "UIComponent";
        }

        public static T Create<T>(UIObject parent, UIComponentSettings settings = null) where T : UIComponent
        {
            if (parent == null)
                throw new NullReferenceException("UIComponent: the given parentObj was null");

            var component = (T)Activator.CreateInstance(typeof(T), parent);
            component.ApplySettings(settings);
            return component;
        }

        protected virtual void ApplySettings(UIComponentSettings settings)
        {
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void FixedTick()
        {
            base.FixedTick();
        }

        public override void LateTick()
        {
            base.LateTick();
        }
    }
}
