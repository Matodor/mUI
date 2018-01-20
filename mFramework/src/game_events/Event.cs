using System;
using mFramework.Saves;

namespace mFramework.GameEvents
{
    public static partial class mGameEvents
    {
        public class EventArgs
        {
            public object Payload { get; set; }
        }

        public class Event : SaveableFields
        {
            public Func<Event, EventArgs, bool> BeforeInvoke { get; set; } = (_, __) => true;
            public OnEvent OnEvent { get; set; } = (s, e) => { };
            
            public readonly string EventKey;

            /*---------SAVEABLE DATA---------*/
            public SaveableInt BeforeEventCounter; 
            public SaveableInt EventCounter; 
            public SaveableBoolean Enabled;
            public SaveableDateTime LastEvent;
            /*---------SAVEABLE DATA---------*/

            public Event(string eventKey) : base($"mSE_{eventKey}")
            {
                EventKey = eventKey;
                Enabled = true;
                LastEvent = DateTime.MinValue;

                Load();
            }

            public void ResetCounters()
            {
                BeforeEventCounter = 0;
                EventCounter = 0;
            }

            public override void BeforeLoad()
            {
                Enabled = true;
            }

            public override void BeforeSave()
            {
            }

            public override string ToString()
            {
                return
                    $"Key={EventKey} BeforeEventCounter={BeforeEventCounter} EventCounter={EventCounter} Enabled={Enabled}";
            }

            public void Invoke(object payload = null)
            {
                if (!Enabled) 
                    return;

                BeforeEventCounter++;
                if (BeforeInvoke(this, new EventArgs
                {
                    Payload = payload
                }))
                {
                    EventCounter++;
                    LastEvent = DateTime.Now;
                    OnEvent(this, new EventArgs
                    {
                        Payload = payload
                    });
                }
            }
        }
    }
}