using mFramework.UI;

namespace mFramework
{
    public static partial class UIExtensions
    {
        public static ScrollView ScrollView(this IView view, ScrollViewSettings settings)
        {
            return view.View<ScrollView>(settings);
        }
    }
}