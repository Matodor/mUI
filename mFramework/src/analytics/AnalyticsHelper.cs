using System;
using System.Linq.Expressions;
using mFramework.UI;
using SimpleJSON;

namespace mFramework.Analytics
{
    public static class AnalyticsHelper
    {
        public static ScreenSession ScreenSession(this IView view, ScreenSession parent = null)
        {
            if (parent == null)
                return new ScreenSession(view, mAnalytics.RootScreenSession);
            return new ScreenSession(view, parent);
        }

        public static void AddField<T>(this JSONObject obj, string prefix, Expression<Func<T>> e)
        {
            if (e.Body is MemberExpression m)
                obj[$"{prefix}_{m.Member.Name}"] = e.Compile().Invoke().ToString();
        }
    }
}