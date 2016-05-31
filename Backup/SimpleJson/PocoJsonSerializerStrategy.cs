namespace SimpleJson
{
    using SimpleJson.Reflection;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
    {
        internal SimpleJson.Reflection.CacheResolver CacheResolver;
        private static readonly string[] Iso8601Format = new string[] { @"yyyy-MM-dd\THH:mm:ss.FFFFFFF\Z", @"yyyy-MM-dd\THH:mm:ss\Z", @"yyyy-MM-dd\THH:mm:ssK" };

        public PocoJsonSerializerStrategy()
        {
            this.CacheResolver = new SimpleJson.Reflection.CacheResolver(new MemberMapLoader(this.BuildMap));
        }

        protected virtual void BuildMap(Type type, SafeDictionary<string, SimpleJson.Reflection.CacheResolver.MemberMap> memberMaps)
        {
            foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                memberMaps.Add(info.Name, new SimpleJson.Reflection.CacheResolver.MemberMap(info));
            }
            foreach (FieldInfo info2 in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                memberMaps.Add(info2.Name, new SimpleJson.Reflection.CacheResolver.MemberMap(info2));
            }
        }

        public virtual object DeserializeObject(object value, Type type)
        {
            object source = null;
            if (value is string)
            {
                string str = value as string;
                if (!string.IsNullOrEmpty(str))
                {
                    if ((type == typeof(DateTime)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(DateTime))))
                    {
                        source = DateTime.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    else if ((type == typeof(Guid)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid))))
                    {
                        source = new Guid(str);
                    }
                    else
                    {
                        source = str;
                    }
                }
                else if (type == typeof(Guid))
                {
                    source = new Guid();
                }
                else if (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid)))
                {
                    source = null;
                }
                else
                {
                    source = str;
                }
            }
            else if (value is bool)
            {
                source = value;
            }
            else if (value == null)
            {
                source = null;
            }
            else if (((value is long) && (type == typeof(long))) || ((value is double) && (type == typeof(double))))
            {
                source = value;
            }
            else if (((value is double) && (type != typeof(double))) || ((value is long) && (type != typeof(long))))
            {
                source = typeof(IConvertible).IsAssignableFrom(type) ? Convert.ChangeType(value, type, CultureInfo.InvariantCulture) : value;
            }
            else
            {
                if (value is IDictionary<string, object>)
                {
                    IDictionary<string, object> dictionary = (IDictionary<string, object>) value;
                    if (ReflectionUtils.IsTypeDictionary(type))
                    {
                        Type type2 = type.GetGenericArguments()[0];
                        Type type3 = type.GetGenericArguments()[1];
                        IDictionary newInstance = (IDictionary) SimpleJson.Reflection.CacheResolver.GetNewInstance(typeof(Dictionary<,>).MakeGenericType(new Type[] { type2, type3 }));
                        foreach (KeyValuePair<string, object> pair in dictionary)
                        {
                            newInstance.Add(pair.Key, this.DeserializeObject(pair.Value, type3));
                        }
                        return newInstance;
                    }
                    source = SimpleJson.Reflection.CacheResolver.GetNewInstance(type);
                    SafeDictionary<string, SimpleJson.Reflection.CacheResolver.MemberMap> dictionary3 = this.CacheResolver.LoadMaps(type);
                    if (dictionary3 == null)
                    {
                        return value;
                    }
                    foreach (KeyValuePair<string, SimpleJson.Reflection.CacheResolver.MemberMap> pair2 in dictionary3)
                    {
                        SimpleJson.Reflection.CacheResolver.MemberMap map = pair2.Value;
                        if (map.Setter != null)
                        {
                            string key = pair2.Key;
                            if (dictionary.ContainsKey(key))
                            {
                                object obj3 = this.DeserializeObject(dictionary[key], map.Type);
                                map.Setter(source, obj3);
                            }
                        }
                    }
                    return source;
                }
                if (!(value is IList<object>))
                {
                    return source;
                }
                IList<object> list = (IList<object>) value;
                IList list2 = null;
                if (type.IsArray)
                {
                    list2 = (IList) Activator.CreateInstance(type, new object[] { list.Count });
                    int num = 0;
                    foreach (object obj4 in list)
                    {
                        list2[num++] = this.DeserializeObject(obj4, type.GetElementType());
                    }
                    return list2;
                }
                if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || typeof(IList).IsAssignableFrom(type))
                {
                    Type type5 = type.GetGenericArguments()[0];
                    list2 = (IList) SimpleJson.Reflection.CacheResolver.GetNewInstance(typeof(List<>).MakeGenericType(new Type[] { type5 }));
                    foreach (object obj5 in list)
                    {
                        list2.Add(this.DeserializeObject(obj5, type5));
                    }
                }
                return list2;
            }
            if (ReflectionUtils.IsNullableType(type))
            {
                return ReflectionUtils.ToNullableType(source, type);
            }
            if ((source == null) && (type == typeof(Guid)))
            {
                return new Guid();
            }
            return source;
        }

        protected virtual object SerializeEnum(Enum p)
        {
            return Convert.ToDouble(p, CultureInfo.InvariantCulture);
        }

        public virtual bool SerializeNonPrimitiveObject(object input, out object output)
        {
            if (!this.TrySerializeKnownTypes(input, out output))
            {
                return this.TrySerializeUnknownTypes(input, out output);
            }
            return true;
        }

        protected virtual bool TrySerializeKnownTypes(object input, out object output)
        {
            bool flag = true;
            if (input is DateTime)
            {
                DateTime time = (DateTime) input;
                output = time.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
                return flag;
            }
            if (input is Guid)
            {
                output = ((Guid) input).ToString("D");
                return flag;
            }
            if (input is Uri)
            {
                output = input.ToString();
                return flag;
            }
            if (input is Enum)
            {
                output = this.SerializeEnum((Enum) input);
                return flag;
            }
            flag = false;
            output = null;
            return flag;
        }

        protected virtual bool TrySerializeUnknownTypes(object input, out object output)
        {
            output = null;
            Type type = input.GetType();
            if (type.FullName == null)
            {
                return false;
            }
            IDictionary<string, object> dictionary = new JsonObject();
            foreach (KeyValuePair<string, SimpleJson.Reflection.CacheResolver.MemberMap> pair in this.CacheResolver.LoadMaps(type))
            {
                if (pair.Value.Getter != null)
                {
                    dictionary.Add(pair.Key, pair.Value.Getter(input));
                }
            }
            output = dictionary;
            return true;
        }
    }
}

