using System;
using System.Collections.Generic;
using mFramework.Saves;

namespace mFramework.GameEvents
{
    public static partial class mGameEvents
    {
        public static int EventsCount => _events.Count;

        public delegate void OnEvent(Event sender, EventArgs eventData);

        private static readonly Dictionary<string, Event> _events;

        static mGameEvents()
        {
            _events = new Dictionary<string, Event>();
            mCore.ApplicationQuit += Save;
            mCore.ApplicationPaused += OnApplicationPaused;
        }

        private static void OnApplicationPaused(bool paused)
        {
            if (paused)
            {
                Save();
            }
        }

        private static void Save()
        {
            _events.Values.Save();
        }

        public static Event GetEvent(string key)
        {
            if (_events.TryGetValue(key, out var @event))
                return @event;
            return null;
        }

        public static bool DetachEvent(string key)
        {
            return _events.Remove(key);
        }

        public static void AttachEvent(Event e)
        {
            if (e.EventKey == null || _events.ContainsKey(e.EventKey))
                return;
            _events.Add(e.EventKey, e);
        }

        public static void InvokeEvent(string key, object payload = null)
        {
            if (!_events.ContainsKey(key))
                return;
            _events[key].Invoke(payload);
        }
    }
}