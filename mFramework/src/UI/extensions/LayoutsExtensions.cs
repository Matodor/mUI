using mFramework.UI.Layouts;

namespace mFramework.UI
{
    public static partial class UIExtensions
    {
        public static FlexboxLayout Flexbox(this IView parentView, FlexboxLayoutProps props)
        {
            return parentView.View<FlexboxLayout>(props);
        }
    }
}