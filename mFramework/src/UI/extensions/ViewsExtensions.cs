namespace mFramework.UI
{
    public static partial class UIExtensions
    {
        public static ScrollView ScrollView(this IView view, ScrollViewProps props)
        {
            return view.View<ScrollView>(props);
        }
    }
}