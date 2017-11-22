namespace mFramework
{
    public interface ISaveableField
    {
        bool SaveValue(ISaveableFieldsBridge bridge);
        ISaveableField LoadValue(ISaveableFieldsBridge bridge);
    }
}
