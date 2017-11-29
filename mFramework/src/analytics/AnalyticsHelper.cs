using System;
using System.Linq.Expressions;
using SimpleJSON;

namespace mFramework.Analytics
{
    public static class AnalyticsHelper
    {
        public static void AddField<T>(this JSONObject obj, string prefix, Expression<Func<T>> e)
        {
            if (e.Body is MemberExpression m)
                obj[$"{prefix}_{m.Member.Name}"] = e.Compile().Invoke().ToString();
        }
    }
}