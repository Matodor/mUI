// TODO
// ивенты последовательно, указывается ИД ивента
// 

using System;
using mFramework.GameEvents;
using mFramework.UI;
using SimpleJSON;
using UnityEngine;

namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        public static ScreenSession RootScreenSession
        {
            get
            {
                if (_rootScreenSession == null)
                    _rootScreenSession = new ScreenSession(mUI.BaseView, null);
                return _rootScreenSession;
            }
        }

        public static readonly string SessionGUID;
        public static readonly string UserGUID;
        public static string Remote_API = "";

        private static ScreenSession _rootScreenSession;
        private static readonly AnalyticsStats _analyticsStats;
        private static readonly MouseEventListener _mouseEventListener;

        static mAnalytics()
        {
            _analyticsStats = new AnalyticsStats();
            _analyticsStats.Load();
            _mouseEventListener = MouseEventListener.Create();

            UserGUID = _analyticsStats.GUID;
            SessionGUID = Guid.NewGuid().ToString();
            
            mGameEvents.AttachEvent(new mGameEvents.Event(AnalyticsEvents.INIT)
            {
                OnEvent = (s, e) =>
                {
                    mCore.Log($"[mAnalytics] Init (UserGUID={UserGUID})");
                    mCore.Log($"[mAnalytics] DeviceInfo: {GetDeviceInfo().ToString()}");

                    if (s.EventCounter == 1)
                    {
                        // send first open app event to analytics
                        // send device info   
                    }
                } 
            });
            mCore.ApplicationQuitEvent += OnQuitEvent;
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

        private static void OnQuitEvent()
        {
            _mouseEventListener.Detach();
            _analyticsStats.Save();

            foreach (var screenSession in RootScreenSession.DeepChild())
            {
                screenSession.Update();
            }

            var session = new JSONObject
            {
                ["type"] = "screen_session",
                ["root"] = RootScreenSession.Session,
                ["device_info"] = GetDeviceInfo()
            };

            AttachSessionInfo(session);
            SendJSON(session);
        }

        public static void AttachSessionInfo(JSONObject json)
        {
            json["session_guid"] = SessionGUID;
            json["user_guid"] = UserGUID;
        }

        public static void SendJSON(JSONObject obj)
        {
            mCore.Log($"Send json: {obj.ToString()}");
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