namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        public static string GUID => _analyticsStats.GUID;

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