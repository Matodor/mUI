using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public interface ISaveableField
    {
        bool SaveValue(ISaveableFieldsBridge bridge);
        ISaveableField LoadValue(ISaveableFieldsBridge bridge);
    }
}
