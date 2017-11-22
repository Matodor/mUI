namespace mFramework
{
    public interface ISaveableFieldsBridge
    {
        string Key { get; }
        IKeyValueStorage Storage { get; }
    }
}
