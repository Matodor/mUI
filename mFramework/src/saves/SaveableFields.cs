﻿using mFramework.concrete;

namespace mFramework.Saves
{
    public abstract class SaveableFields
    {
        public static IKeyValueStorage Storage { get; } = PlayerPrefsStorage.Instance;
        public string Key { get; }

        protected SaveableFields(string key)
        {
            Key = key;
        }

        public virtual void OnReset() { }
        public virtual void BeforeSave() { }
        public virtual void BeforeLoad() { }

        public virtual void AfterSave() { }
        public virtual void AfterLoad() { }

        public void Save()
        {
            BeforeSave();

            var type = GetType();
            var fieldsInfo = mCore.GetCachedFields(type);

            for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
            {
                if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                {
                    continue;
                }

                var key = GetKey(Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                var saveableField = (ISaveableField) fieldsInfo.CachedFields[i].Getter(this);
                var bridge = new SaveableFieldsBridge(key, Storage);
                saveableField.SaveValue(bridge);
            }

            AfterSave();
        }

        public void Load()
        {
            BeforeLoad();

            var type = GetType();
            var fieldsInfo = mCore.GetCachedFields(type);

            for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
            {
                if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                {
                    continue;
                }

                var key = GetKey(Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                if (Storage.HasKey(key))
                {
                    var saveableField = (ISaveableField) fieldsInfo.CachedFields[i].Getter(this);
                    var bridge = new SaveableFieldsBridge(key, Storage);
                    var newValue = saveableField.LoadValue(bridge);
                    fieldsInfo.CachedFields[i].Setter(this, newValue);
                }
            }

            AfterLoad();
        }

        private static string GetKey(string fieldsKey, string fieldName)
        {
            return KnuthHash.CalculateHash(fieldsKey + fieldName).ToString();
        }
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
