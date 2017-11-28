using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SimpleJSON;

namespace mFramework.RemoteSettings
{
    public static class mRemoteSettings
    {
        public static event Action Updated
        {
            add
            {
                _updated += value;

                if (!_requested)
                {
                    ForceUpdate();
                    _requested = true;
                }
            }
            remove
            {
                _updated -= value;

                if (!_requested)
                {
                    ForceUpdate();
                    _requested = true;
                }
            }
        }

        public static DateTime LastUpdated { get; private set; }
        public static JSONNode JSON { get; private set; }

        public static string RemoteUrl = "";
        public static int SettingsCount => _settings.Count;

        private static event Action _updated;
        private static readonly Dictionary<string, JSONNode> _settings;
        private static bool _requested;

        static mRemoteSettings()
        {
            JSON = null;
            _requested = false;
            _settings = new Dictionary<string, JSONNode>();
        }

        public static bool GetBoolean(string key, bool failureValue = false)
        {
            if (HasKey(key) && bool.TryParse(_settings[key].Value, out var value))
                return value;
            return failureValue;
        }

        public static int GetInt(string key, int failureValue = 0)
        {
            if (HasKey(key) && int.TryParse(_settings[key].Value, out var value))
                return value;
            return failureValue;
        }

        public static string GetString(string key, string failureValue = "")
        {
            if (HasKey(key))
                return _settings[key].Value;
            return failureValue;
        }

        public static float GetFloat(string key, float failureValue = 0f)
        {
            if (HasKey(key) && float.TryParse(_settings[key].Value, out var value))
                return value;
            return failureValue;
        }

        public static bool HasKey(string key)
        {
            return _settings.ContainsKey(key);
        }

        public static string[] Keys()
        {
            return _settings.Keys.ToArray();
        }

        public static void ForceUpdate()
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri(RemoteUrl));
            }
        }

        private static void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                return;

            LastUpdated = DateTime.Now;
            JSON = SimpleJSON.JSON.Parse(e.Result);

            if (JSON == null || JSON["settings"] == null || !JSON["settings"].IsArray)
                return;

            foreach (var property in JSON["settings"].Children)
            {
                if (property["key"] == null || property["value"] == null)
                    continue;

                // not used
                // var settingType = property["type"].Value;
                var settingKey = property["key"].Value;
                var settingValue = property["value"];

                _settings.Add(settingKey, settingValue);
            }

            _updated?.Invoke();
        }
    }
}