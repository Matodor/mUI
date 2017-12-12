using System;
using System.Linq;
using mFramework.GameEvents;
using SimpleJSON;
using UnityEngine;

namespace mFramework.Analytics
{
    public static class mAnalytics
    {
        /*public static ScreenSession RootScreenSession
        {
            get
            {
                if (_rootScreenSession == null)
                    _rootScreenSession = new ScreenSession(mUI.BaseView, null);
                return _rootScreenSession;
            }
        }*/

        public static readonly string UserGUID;
        public static readonly string SessionGUID;
        public static string Remote_API = "";

        private static readonly JSONArray _data = new JSONArray();

        //private static ScreenSession _rootScreenSession;
        private static JSONObject _analyticsData;
        private static readonly DateTime _startSession;
        private static readonly AnalyticsStats _analyticsStats;
        private static readonly MouseEventListener _mouseEventListener;
        private static string[] _ignoreEvents;

        static mAnalytics()
        {
            _ignoreEvents = new string[0];
            _analyticsStats = new AnalyticsStats();
            _analyticsStats.Load();
            
            UserGUID = _analyticsStats.GUID;
            SessionGUID = Guid.NewGuid().ToString();
            
            _mouseEventListener = MouseEventListener.Create();
            _analyticsData = CreateAnalyticsData();
            _analyticsData["device"] = GetDeviceInfo();
            _startSession = DateTime.Now;
            
            mGameEvents.AttachEvent(new mGameEvents.Event(AnalyticsEvents.INIT)
            {
                OnEvent = (s, e) =>
                {
                    mCore.Log($"[mAnalytics] Init UserGUID={UserGUID} SessionGUID={SessionGUID}");
                    mCore.Log($"[mAnalytics] DeviceInfo: {GetDeviceInfo().ToString()}");

                    if (s.EventCounter == 1)
                    {
                        // send first open app event to analytics
                        // send device info   
                    }
                }
            });

            CustomEvent("start_session");
            Flush();

            mCore.ApplicationQuitEvent += OnQuitEvent;
        }

        public static void Flush()
        {
            _data.Add(_analyticsData);
            _analyticsData = CreateAnalyticsData();
        }

        private static JSONObject CreateAnalyticsData()
        {
            return new JSONObject()
            {
                ["session_guid"] = SessionGUID,
                ["user_guid"] = UserGUID,
                ["events"] = new JSONArray()
            };
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

            jsonObject.AddField("is", () => SystemInfo.supportsVibration);
            jsonObject.AddField("is", () => SystemInfo.supportsGyroscope);
            jsonObject.AddField("is", () => SystemInfo.supportsAccelerometer);
            jsonObject.AddField("is", () => SystemInfo.supportsLocationService);
            jsonObject.AddField("is", () => SystemInfo.supportsAudio);

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

        public static void CustomEvent(string eventName, JSONNode payload = null)
        {
            if (_ignoreEvents.Contains(eventName))
                return;
            
            var e = new JSONObject
            {
                ["eventName"] = eventName,
                ["time"] = (DateTime.Now - _startSession).TotalSeconds.ToString("F2"),
                ["payload"] = payload
            };
            _analyticsData["events"].AsArray.Add(e);

            if (_analyticsData["events"].Count >= 10)
                Flush();
        }

        private static void OnQuitEvent()
        {
            _mouseEventListener.Detach();
            _analyticsStats.Save();

            CustomEvent("end_session");
            Flush();

            mCore.Log(_data.ToString());
            mCore.ApplicationQuitEvent -= OnQuitEvent;
        }
    }
}