﻿using mFramework.UI;
using mFramework.UI.Layouts;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static FlexboxLayout Flexbox(this IView parentView, FlexboxLayoutSettings settings)
        {
            return parentView.View<FlexboxLayout>(settings);
        }
    }
}