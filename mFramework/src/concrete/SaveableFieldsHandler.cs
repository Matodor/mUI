using System;
using System.Collections.Generic;

namespace mFramework
{
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

        public static void AddFields(IEnumerable<SaveableFields> fields)
        {
            foreach (var f in fields)
            {
                AddFields(f);
            }
        }

        public static void AddFields(SaveableFields container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (_fields.ContainsKey(container.Key))
                throw new Exception("SaveableFieldsHandler already has fields container with same key");

            _fields.Add(container.Key, container);
            
            //var fieldsInfo = mCore.GetCachedFields(container.GetType());

            /*for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
            {
                if (fieldsInfo.CachedFields[i].FieldInfo.FieldType.IsArray)
                {
                    //throw new Exception($"Class '{container}' has array field");
                }

                if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                {
                    //throw new Exception(
                    //    $"Class '{container}' has field different type ({fieldsInfo.CachedFields[i].FieldInfo.FieldType}) from ISaveableField");
                }
            }*/
        }

        public static bool Save()
        {
            foreach (var saveableFieldsContainer in _fields)
            {
                saveableFieldsContainer.Value.BeforeSave();

                var type = saveableFieldsContainer.Value.GetType();
                var fieldsInfo = mCore.GetCachedFields(type);

                for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
                {
                    if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                    {
                        continue;
                    }

                    var key = Key(saveableFieldsContainer.Value.Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                    var saveableField = (ISaveableField)fieldsInfo.CachedFields[i].Getter(saveableFieldsContainer.Value);
                    var bridge = new SaveableFieldsBridge(key, PlayerPrefsStorage.Instance);

                    saveableField.SaveValue(bridge);
                }

                saveableFieldsContainer.Value.AfterSave();
            }
            _storage.Save();
            return true;
        }

        public static bool Load()
        {
            foreach (var saveableFieldsContainer in _fields)
            {
                saveableFieldsContainer.Value.BeforeLoad();

                var type = saveableFieldsContainer.Value.GetType();
                var fieldsInfo = mCore.GetCachedFields(type);

                for (int i = 0; i < fieldsInfo.CachedFields.Length; i++)
                {
                    if (!typeof(ISaveableField).IsAssignableFrom(fieldsInfo.CachedFields[i].FieldInfo.FieldType))
                    {
                        continue;
                    }

                    var key = Key(saveableFieldsContainer.Value.Key, fieldsInfo.CachedFields[i].FieldInfo.Name);
                    var saveableField = (ISaveableField)fieldsInfo.CachedFields[i].Getter(saveableFieldsContainer.Value);
                    var bridge = new SaveableFieldsBridge(key, PlayerPrefsStorage.Instance);
                    var newValue = saveableField.LoadValue(bridge);
                    fieldsInfo.CachedFields[i].Setter(saveableFieldsContainer.Value, newValue);
                }
                saveableFieldsContainer.Value.AfterLoad();
            }

            return true;
        }

        internal static string Key(string fieldsClassKey, string fieldName)
        {
            return $"{fieldsClassKey}_{fieldsClassKey.GetHashCode()}_{fieldName}_{fieldName.GetHashCode()}";
        }
    }
}
