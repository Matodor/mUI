using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mUIApp
{
    public interface IKeyStorage
    {
        event Action<mUIKeyStorage> OnBeforeSave;
        event Action<mUIKeyStorage> OnAfterSave;
        event Action<mUIKeyStorage> OnBeforeLoad;
        event Action<mUIKeyStorage> OnAfterLoad;

        void Save();
        bool Load();
        T GetValue<T>(string key) where T : struct;
        void SetValue<T>(string key, T value) where T : struct;
    }
}
