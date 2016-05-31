namespace SimpleJson.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    public class ReflectionUtils
    {
        public static Attribute GetAttribute(MemberInfo info, Type type)
        {
            if (((info != null) && (type != null)) && Attribute.IsDefined(info, type))
            {
                return Attribute.GetCustomAttribute(info, type);
            }
            return null;
        }

        public static Attribute GetAttribute(Type objectType, Type attributeType)
        {
            if (((objectType != null) && (attributeType != null)) && Attribute.IsDefined(objectType, attributeType))
            {
                return Attribute.GetCustomAttribute(objectType, attributeType);
            }
            return null;
        }

        public static bool IsNullableType(Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsTypeDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            if (!type.IsGenericType)
            {
                return false;
            }
            return (type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public static bool IsTypeGenericeCollectionInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if ((genericTypeDefinition != typeof(IList<>)) && (genericTypeDefinition != typeof(ICollection<>)))
            {
                return (genericTypeDefinition == typeof(IEnumerable<>));
            }
            return true;
        }

        public static object ToNullableType(object obj, Type nullableType)
        {
            if (obj != null)
            {
                return Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
            }
            return null;
        }
    }
}

