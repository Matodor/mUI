﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mFramework
{
    public abstract class SaveableFields
    {
        public string Key { get; }

        protected SaveableFields(string key)
        {
            Key = key;
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

    public static class SaveableFieldsHandler
    {
       
        private static readonly IKeyValueStorage _storage;
        private static readonly Dictionary<string, SaveableFields> _fields;

        static SaveableFieldsHandler()
        {
            _storage = PlayerPrefsStorage.Instance;
            _fields = new Dictionary<string, SaveableFields>();
        }

        public static bool RemoveFields(SaveableFields fields)
        {
            if (!_fields.ContainsKey(fields.Key))
                return false;
            return _fields.Remove(fields.Key);
        }

        public static void AddFields(SaveableFields fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            if (_fields.ContainsKey(fields.Key))
                throw new Exception("SaveableFieldsHandler already has fields with same key");

            _fields.Add(fields.Key, fields);
            var fieldsInfo = mCore.GetCachedFields(fields.GetType());

            for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
            {
                if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                    throw new Exception(string.Format(
                        "Class '{0}' has field different type ({1}) from ISaveableField", fields,
                        fieldsInfo.CachedFields[i].FieldInfo.FieldType));
            }
        }

        public static bool Save()
        {
            foreach (var saveableFieldsContainer in _fields)
            {
                var type = saveableFieldsContainer.Value.GetType();
                var fieldsInfo = mCore.GetCachedFields(type);
                for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
                {
                    var key = Key(saveableFieldsContainer.Value.Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                    var saveableField = (ISaveableField)fieldsInfo.CachedFields[i].Getter(saveableFieldsContainer.Value);
                    var bridge = new SaveableFieldsBridge(key, PlayerPrefsStorage.Instance);

                    saveableField.SaveValue(bridge);
                }
            }
            _storage.Save();
            return true;
        }

        public static bool Load()
        {
            foreach (var saveableFieldsContainer in _fields)
            {
                var type = saveableFieldsContainer.Value.GetType();
                var fieldsInfo = mCore.GetCachedFields(type);
                for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
                {
                    var key = Key(saveableFieldsContainer.Value.Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                    var saveableField = (ISaveableField)fieldsInfo.CachedFields[i].Getter(saveableFieldsContainer.Value);
                    var bridge = new SaveableFieldsBridge(key, PlayerPrefsStorage.Instance);
                    var newValue = saveableField.LoadValue(bridge);
                    fieldsInfo.CachedFields[i].Setter(saveableFieldsContainer.Value, newValue);
                }
            }

            return true;
        }

        public static string Key(string fieldsClassKey, string fieldName)
        {
            return string.Format("{0}_{1}_{2}_{3}", fieldsClassKey, fieldsClassKey.GetHashCode(),
                fieldName, fieldName.GetHashCode());
        }
    }
}
