using mFramework.GameEvents;
using UnityEngine;

namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        public static string GUID => _analyticsStats.GUID;
        public static string RemoteUrl = "";

        private static readonly AnalyticsStats _analyticsStats;

        static mAnalytics()
        {
            _analyticsStats = new AnalyticsStats();
            _analyticsStats.Load();

            mGameEvents.AttachEvent(new mGameEvents.Event(AnalyticsEvents.INIT)
            {
                OnEvent = (s, e) =>
                {
                    mCore.Log($"[mAnalytics] Init (GUID={GUID})");

                    if (s.EventCounter == 1)
                    {
                        // send first open app event to analytics
                        //Application.installMode
                        //Application.platform
                        //Application.systemLanguage
                        //Application.version
                        //Application.unityVersion
                        //Application.backgroundLoadingPriority=

                        //SystemInfo.deviceModel
                        //SystemInfo.deviceName
                        //SystemInfo.deviceType
                        //SystemInfo.operatingSystem
                        //SystemInfo.operatingSystemFamily
                        //SystemInfo.

                        //Screen.dpi
                    }
                } 
            });
            mCore.ApplicationQuitEvent += SaveAnalytics;
        }

        public static void Init()
        {
            mGameEvents.InvokeEvent(AnalyticsEvents.INIT);
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