using System;
using System.Collections.Generic;
using mFramework.Saves;

namespace mFramework.GameEvents
{
    public static partial class mGameEvents
    {
        public static int EventsCount => _events.Count;

        public delegate void OnEvent(Event sender, EventArgs eventData);

        private static readonly Dictionary<Enum, Event> _events;

        static mGameEvents()
        {
            _events = new Dictionary<Enum, Event>();
            mCore.ApplicationQuitEvent += SaveGameEvents;
        }

        private static void SaveGameEvents()
        {
            _events.Values.Save();
        }

        public static Event GetEvent(Enum key)
        {
            if (_events.TryGetValue(key, out var @event))
                return @event;
            return null;
        }

        public static bool DetachEvent(Enum key)
        {
            return _events.Remove(key);
        }

        public static void AttachEvent(Event e)
        {
            if (e.EventKey == null || _events.ContainsKey(e.EventKey))
                return;
            _events.Add(e.EventKey, e);
        }

        public static void InvokeEvent(Enum key, object payload = null)
        {
            if (!_events.ContainsKey(key))
                return;
            _events[key].Invoke(payload);
        }
    }
}