using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mUIApp.Views
{
    interface UIRect
    {
        float Width { get; }
        float Height { get; }
        float PureWidth { get; }
        float PureHeight { get; }

        float LeftAnchor { get; }
        float RightAnchor { get; }
        float TopAnchor { get; }
        float BottomAnchor { get; }
    }
}
