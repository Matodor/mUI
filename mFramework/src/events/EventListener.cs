namespace mFramework
{
    public abstract class EventListener : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }
        
        private static ulong _guid;

        protected EventListener()
        {
            GUID = ++_guid;
        }

        public abstract void Detach();
    }
}
