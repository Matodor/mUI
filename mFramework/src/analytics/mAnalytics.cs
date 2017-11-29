using mFramework.GameEvents;
using SimpleJSON;
using UnityEngine;

namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        public static string GUID => _analyticsStats.GUID;
        public static string Remote_API = "";

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
                    mCore.Log($"[mAnalytics] DeviceInfo: {GetDeviceInfo().ToString()}");

                    if (s.EventCounter == 1)
                    {
                        // send first open app event to analytics
                        // send device info   
                    }
                } 
            });
            mCore.ApplicationQuitEvent += SaveAnalytics;
        }

        public static JSONObject GetDeviceInfo()
        {
            var jsonObject = new JSONObject();
            jsonObject.AddField("app", () => Application.installerName);
            jsonObject.AddField("app", () => Application.installMode);
            jsonObject.AddField("app", () => Application.platform);
            jsonObject.AddField("app", () => Application.systemLanguage);
            jsonObject.AddField("app", () => Application.version);
            jsonObject.AddField("app", () => Application.unityVersion);
            jsonObject.AddField("app", () => Application.buildGUID);

            jsonObject.AddField("sys", () => SystemInfo.deviceModel);
            jsonObject.AddField("sys", () => SystemInfo.deviceName);
            jsonObject.AddField("sys", () => SystemInfo.deviceType);
            jsonObject.AddField("sys", () => SystemInfo.operatingSystem);
            jsonObject.AddField("sys", () => SystemInfo.operatingSystemFamily);

            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceID);
            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceName);
            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceName);
            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceVendor);
            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceVendorID);
            jsonObject.AddField("sys", () => SystemInfo.graphicsDeviceVersion);
            jsonObject.AddField("sys", () => SystemInfo.graphicsMemorySize);

            jsonObject.AddField("sys", () => SystemInfo.processorCount);
            jsonObject.AddField("sys", () => SystemInfo.processorFrequency);
            jsonObject.AddField("sys", () => SystemInfo.processorType);
            jsonObject.AddField("sys", () => SystemInfo.systemMemorySize);

            jsonObject.AddField("screen", () => Screen.dpi);
            jsonObject.AddField("screen", () => Screen.width);
            jsonObject.AddField("screen", () => Screen.height);

            return jsonObject;
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
            // post
            // new WebClient().UploadValues(Remote_API, );
        }

        public static void SendEvent(string eventKey)
        {
            
        }
    }
}