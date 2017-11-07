using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public interface IKeyValueStorage
    {
        bool GetInt(string key, out int value);
        bool GetFloat(string key, out float value);
        bool GetString(string key, out string value);

        bool SetInt(string key, int value);
        bool SetFloat(string key, float value);
        bool SetString(string key, string value);

        void Save();
        void Clear();
    }
}
