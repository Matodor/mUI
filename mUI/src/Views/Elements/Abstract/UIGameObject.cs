using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Views
{
    public interface UIGameObject
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        BaseView ParentView { get; }
    }
}
