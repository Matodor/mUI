using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mUIApp
{
    public interface IKeyStorage
    {
        void Save();
        void Load();
        T GetValue<T>(string key) where T : struct;
        void SetValue<T>(string key, T value) where T : struct;
    }
}
