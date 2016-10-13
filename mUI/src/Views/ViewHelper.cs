﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Views
{
    public static class ViewHelper
    {
        public delegate void InitView(BaseView view, Transform parenTransform);

        public static InitView InitViewCallback { get; }

        static ViewHelper()
        {
            var methodInfo = typeof (BaseView).GetMethod("InitBase", BindingFlags.NonPublic | BindingFlags.Instance);
            InitViewCallback = (InitView) Delegate.CreateDelegate(typeof (InitView), methodInfo);
        }

        public static PartialView CreatePartial<T>(this BaseView view, string viewName = "partialView") where T : PartialView, new()
        {
            var partialView = new T();
            InitViewCallback(partialView, view.Transform);
            partialView.GameObject.name = viewName;
            partialView.ParentView = view;
            view.AddChildView(partialView);
            return partialView;
        }
    }
}
