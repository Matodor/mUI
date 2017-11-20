using System;

namespace mFramework.GameFunnels
{
    public static partial class mGameFunnels
    {
        public class EventArgs
        {
            public object Payload { get; set; }
        }

        public class Event : SaveableFields
        {
            public Func<Event, bool> BeforeInvoke { get; set; } = _ => true;
            public OnEvent OnEvent { get; set; } = (s, e) => { };
            
            public readonly Enum EventKey;

            /*---------SAVEABLE DATA---------*/
            public SaveableInt BeforeEventCounter; 
            public SaveableInt EventCounter; 
            public SaveableBoolean Enabled;
            /*---------SAVEABLE DATA---------*/

            public Event(Enum eventKey) : base($"mSaveableEvent_{eventKey}")
            {
                EventKey = eventKey;
                Enabled = true;

                SaveableFieldsHandler.AddFields(this);
                SaveableFieldsHandler.Load(this);
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
                if (BeforeInvoke(this))
                {
                    EventCounter++;
                    OnEvent(this, new EventArgs
                    {
                        Payload = payload
                    });
                }
            }
        }
    }
}