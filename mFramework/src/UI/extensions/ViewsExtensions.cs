using mFramework.UI;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static ScrollView Container(this IView view, ScrollViewSettings settings)
        {
            return view.View<ScrollView>(settings);
        }
    }
}