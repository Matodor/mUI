using System;
using mFramework.GameEvents;
using mFramework.UI;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        internal static JSONObject JSONToSend { get; private set; }

        public static string GUID => _analyticsStats.GUID;
        public static string Remote_API = "";

        private static readonly AnalyticsStats _analyticsStats;
        private static readonly MouseEventListener _mouseEventListener;

        static mAnalytics()
        {
            _analyticsStats = new AnalyticsStats();
            _analyticsStats.Load();
            _mouseEventListener = MouseEventListener.Create();

            JSONToSend = new JSONObject();
            
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

        public static void Init(Type[] viewToScreenSession)
        {
            mGameEvents.InvokeEvent(AnalyticsEvents.INIT);
            ScreenSession.ViewsTypes = viewToScreenSession;
            var screenSession = new ScreenSession(mUI.BaseView);
        }

        private static void SaveAnalytics()
        {
            _mouseEventListener.Detach();
            _analyticsStats.Save();
        }

        public static void Flush()
        {
            
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