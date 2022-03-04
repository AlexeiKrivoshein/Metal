using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx.Validation
{
    public static class Validator
    {
        private static Dictionary<Type, object> _defaultValueMap = new Dictionary<Type, object>();

        public static bool Validate(ValidationMapTuple ValidationTuple, object dtoObject, out string error)
        {
            var value = ValidationTuple.Property.GetValue(dtoObject, null);
            var type = ValidationTuple.Property.PropertyType;
            var errorSB = new StringBuilder();
            var isValid = true;

            foreach (var attribute in ValidationTuple.ValidationAttributes)
            {
                if(attribute is RequiredAttribute)
                {
                    object @default = null;
                    if (!_defaultValueMap.TryGetValue(type, out @default))
                    {
                        @default = GetDefaultValue(type);
                        _defaultValueMap[type] = @default;
                    }

                    dynamic castedValue = Convert.ChangeType(value, type);
                    dynamic castedDefault = Convert.ChangeType(@default, type);

                    if (castedValue == castedDefault)
                    {
                        errorSB.AppendLine(attribute.Error);
                        isValid = false;
                    }
                }
                else if (attribute is NumberRangeAttribute numberRange)
                {
                    double number = 0;
                    bool parsed = true;
                    if (value is sbyte sByteValue)
                    {
                        number = sByteValue;
                    }
                    else if (value is byte byteValue)
                    {
                        number = byteValue;
                    }
                    else if (value is short shortValue)
                    {
                        number = shortValue;
                    }
                    else if (value is ushort ushortValue)
                    {
                        number = ushortValue;
                    }
                    else if (value is int intValue)
                    {
                        number = intValue;
                    }
                    else if (value is uint uIntValue)
                    {
                        number = uIntValue;
                    }
                    else if (value is long longValue)
                    {
                        number = longValue;
                    }
                    else if (value is ulong uLongValue)
                    {
                        number = uLongValue;
                    }
                    else if (value is float floatValue)
                    {
                        number = floatValue;
                    }
                    else if (value is double doubleValue)
                    {
                        number = doubleValue;
                    }
                    else if (value is decimal decimalValue)
                    {
                        number = (double)decimalValue;
                    }
                    else
                    {
                        errorSB.AppendLine(attribute.Error);
                        isValid = false;
                        parsed = false;
                    }

                    if(parsed && 
                      (number < numberRange.From || number > numberRange.To))
                    {
                        errorSB.AppendLine(attribute.Error);
                        isValid = false;
                    }
                }
            }

            error = errorSB.ToString();
            return isValid;
        }

        public static bool TryCast<T>(ref T t, object o)
        {
            if (
                o == null
                || !typeof(T).IsAssignableFrom(o.GetType())
                )
                return false;
            t = (T)o;
            return true;
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
