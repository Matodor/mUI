using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
        public FieldInfo FieldInfo { get; }
        public LateBoundFieldSet Setter { get; }
        public LateBoundFieldGet Getter { get; }

        public CachedFieldInfo(FieldInfo fieldInfo, LateBoundFieldSet setter, LateBoundFieldGet getter)
        {
            FieldInfo = fieldInfo;
            Getter = getter;
            Setter = setter;
        }
    }
}
