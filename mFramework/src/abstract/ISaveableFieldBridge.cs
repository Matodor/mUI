using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public interface ISaveableFieldsBridge
    {
        string Key { get; }
        IKeyValueStorage Storage { get; }
    }
}
