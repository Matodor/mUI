using System;
using mFramework.Saves;

namespace mFramework.Analytics
{
    internal class AnalyticsStats : SaveableFields
    {
        public SaveableString GUID;

        public AnalyticsStats() : base("mAnalytics_stats")
        {
            GUID = null;
        }

        public override void BeforeLoad()
        {
            if (string.IsNullOrWhiteSpace(GUID))
                GUID = Guid.NewGuid().ToString();
        }

        public override void AfterLoad()
        {
            if (string.IsNullOrWhiteSpace(GUID))
                GUID = Guid.NewGuid().ToString();
        }
    }
}