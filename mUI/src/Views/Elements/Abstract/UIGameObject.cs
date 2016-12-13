using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Animations;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Views
{
    public interface UIGameObject
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        BaseView ParentView { get; }
        List<mUIAnimation> Animations { get; }

        void Destroy();
        void OnRotate();
        void OnTranslate();
        void OnScale();
        event Action<UIGameObject> OnRotateEvent;
        event Action<UIGameObject> OnTranslateEvent;
        event Action<UIGameObject> OnScaleEvent;
    }
}
