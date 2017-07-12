namespace mFramework
{
    public abstract class SaveableFields
    {
        public string Key { get; }

        protected SaveableFields(string key)
        {
            Key = key;
        }

        public virtual void BeforeSave() { }
        public virtual void BeforeLoad() { }

        public virtual void AfterSave() { }
        public virtual void AfterLoad() { }
    }

    public struct SaveableFieldsBridge : ISaveableFieldsBridge
    {
        public string Key { get; }
        public IKeyValueStorage Storage { get; }

        public SaveableFieldsBridge(string key, IKeyValueStorage storage)
        {
            Key = key;
            Storage = storage;
        }
    }
}
