using System;
using mFramework.Saves;
using UnityEngine;

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

    public static class mAnalytics
    {
        private static readonly AnalyticsStats _analyticsStats;

        static mAnalytics()
        {
            _analyticsStats = new AnalyticsStats();
            _analyticsStats.Load();

            mCore.ApplicationQuitEvent += SaveAnalytics;
        }

        private static void SaveAnalytics()
        {
            _analyticsStats.Save();
        }

        public static void SendEvent(string eventKey, object data)
        {
            
        }

        public static void SendEvent(string eventKey)
        {
            
        }
    }
}