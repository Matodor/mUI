using System;
using System.Globalization;

namespace mFramework
{
    // int
    public struct SaveableInt : ISaveableField
    {
        public int Value;

        public static implicit operator int(SaveableInt field)
        {
            return field.Value;
        }

        public static implicit operator SaveableInt(int value)
        {
            return new SaveableInt {Value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetInt(bridge.Key, Value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            int value;
            if (bridge.Storage.GetInt(bridge.Key, out value))
                return new SaveableInt {Value = value};
            return new SaveableInt { Value = default(int) };
        }
    }

    // float
    public struct SaveableFloat : ISaveableField
    {
        public float Value;

        public static implicit operator float(SaveableFloat field)
        {
            return field.Value;
        }

        public static implicit operator SaveableFloat(float value)
        {
            return new SaveableFloat {Value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetFloat(bridge.Key, Value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            float value;
            if (bridge.Storage.GetFloat(bridge.Key, out value))
                return new SaveableFloat {Value = value};
            return new SaveableFloat { Value = default(float) };
        }
    }

    // string
    public struct SaveableString : ISaveableField
    {
        public string Value;

        public static implicit operator string(SaveableString field)
        {
            return field.Value;
        }

        public static implicit operator SaveableString(string value)
        {
            return new SaveableString { Value = value };
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            string value;
            if (bridge.Storage.GetString(bridge.Key, out value))
                return new SaveableString { Value = value };
            return new SaveableString { Value = default(string) };
        }
    }

    // ulong
    public struct SaveableUInt64 : ISaveableField
    {
        public ulong Value;

        public static implicit operator ulong(SaveableUInt64 field)
        {
            return field.Value;
        }

        public static implicit operator SaveableUInt64(ulong value)
        {
            return new SaveableUInt64 { Value = value };
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(Value));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            string value;
            if (bridge.Storage.GetString(bridge.Key, out value))
                return new SaveableUInt64 {Value = Convert.ToUInt64(value)};
            return new SaveableUInt64 { Value = default(ulong) };
        }
    }

    public struct SaveableDecimal : ISaveableField
    {
        public decimal Value;

        public static implicit operator decimal(SaveableDecimal field)
        {
            return field.Value;
        }

        public static implicit operator SaveableDecimal(decimal value)
        {
            return new SaveableDecimal { Value = value };
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(Value, CultureInfo.InvariantCulture));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            string value;
            if (bridge.Storage.GetString(bridge.Key, out value))
                return new SaveableDecimal { Value = Convert.ToDecimal(value, CultureInfo.InvariantCulture) };
            return new SaveableDecimal { Value = default(decimal) };
        }
    }
}
