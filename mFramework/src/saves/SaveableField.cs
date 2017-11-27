using System;
using System.Globalization;

namespace mFramework.Saves
{
    public struct SaveableDateTime : ISaveableField
    {
        private DateTime _value;

        public override string ToString()
        {
            return _value.ToString("G");
        }

        public static implicit operator DateTime(SaveableDateTime field)
        {
            return field._value;
        }

        public static implicit operator SaveableDateTime(DateTime value)
        {
            return new SaveableDateTime {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(_value.Ticks));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetString(bridge.Key, out var value))
            {
                try
                {
                    return new SaveableDateTime {_value = new DateTime(Convert.ToInt64(value)) };
                }
                catch
                {
                    return this;
                }
            }
            return this;
        }

    }

    public struct SaveableBoolean : ISaveableField
    {
        private bool _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator bool(SaveableBoolean field)
        {
            return field._value;
        }

        public static implicit operator SaveableBoolean(bool value)
        {
            return new SaveableBoolean {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetInt(bridge.Key, _value ? 1 : 0);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetInt(bridge.Key, out var value))
                return new SaveableBoolean { _value = value == 1 };
            return this;
        }
    }

    public struct SaveableInt : ISaveableField
    {
        private int _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator int(SaveableInt field)
        {
            return field._value;
        }

        public static implicit operator SaveableInt(int value)
        {
            return new SaveableInt {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetInt(bridge.Key, _value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetInt(bridge.Key, out var value))
                return new SaveableInt { _value = value };
            return this;
        }
    }

    public struct SaveableFloat : ISaveableField
    {
        private float _value;

        public override string ToString()
        {
            return _value.ToString("G");
        }

        public static implicit operator float(SaveableFloat field)
        {
            return field._value;
        }

        public static implicit operator SaveableFloat(float value)
        {
            return new SaveableFloat {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetFloat(bridge.Key, _value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetFloat(bridge.Key, out var value))
                return new SaveableFloat {_value = value};
            return this;
        }
    }

    public struct SaveableString : ISaveableField
    {
        private string _value;

        public override string ToString()
        {
            return _value;
        }

        public static implicit operator string(SaveableString field)
        {
            return field._value;
        }

        public static implicit operator SaveableString(string value)
        {
            return new SaveableString {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, _value);
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetString(bridge.Key, out var value))
                return new SaveableString { _value = value };
            return this;
        }
    }

    public struct SaveableInt64 : ISaveableField
    {
        private long _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator long(SaveableInt64 field)
        {
            return field._value;
        }

        public static implicit operator SaveableInt64(long value)
        {
            return new SaveableInt64 { _value = value };
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(_value));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetString(bridge.Key, out var value))
            {
                try
                {
                    return new SaveableInt64 { _value = Convert.ToInt64(value) };
                }
                catch
                {
                    return this;
                }
            }
            return this;
        }
    }


    public struct SaveableUInt64 : ISaveableField
    {
        private ulong _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator ulong(SaveableUInt64 field)
        {
            return field._value;
        }

        public static implicit operator SaveableUInt64(ulong value)
        {
            return new SaveableUInt64 {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(_value));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetString(bridge.Key, out var value))
            {
                try
                {
                    return new SaveableUInt64 { _value = Convert.ToUInt64(value) };
                }
                catch
                {
                    return this;
                }
            }
            return this;
        }
    }

    public struct SaveableDecimal : ISaveableField
    {
        private decimal _value;

        public override string ToString()
        {
            return _value.ToString("G");
        }

        public static implicit operator decimal(SaveableDecimal field)
        {
            return field._value;
        }

        public static implicit operator SaveableDecimal(decimal value)
        {
            return new SaveableDecimal {_value = value};
        }

        public bool SaveValue(ISaveableFieldsBridge bridge)
        {
            return bridge.Storage.SetString(bridge.Key, Convert.ToString(_value, CultureInfo.InvariantCulture));
        }

        public ISaveableField LoadValue(ISaveableFieldsBridge bridge)
        {
            if (bridge.Storage.GetString(bridge.Key, out var value))
            {
                try
                {
                    return new SaveableDecimal { _value = Convert.ToDecimal(value, CultureInfo.InvariantCulture) };
                }
                catch
                {
                    return this;
                }
            }
            return this;
        }
    }
}
