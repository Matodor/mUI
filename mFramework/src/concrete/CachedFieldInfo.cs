using System.Reflection;

namespace mFramework
{
    public class CachedFieldsInfo
    {
        public readonly CachedFieldInfo[] CachedFields;

        public CachedFieldsInfo(int fieldsCount)
        {
            CachedFields = new CachedFieldInfo[fieldsCount];
        }
    }

    public class CachedFieldInfo
    {
        public readonly FieldInfo FieldInfo;
        public readonly LateBoundFieldSet Setter;
        public readonly LateBoundFieldGet Getter;

        public CachedFieldInfo(FieldInfo fieldInfo, LateBoundFieldSet setter, LateBoundFieldGet getter)
        {
            FieldInfo = fieldInfo;
            Getter = getter;
            Setter = setter;
        }
    }
}
