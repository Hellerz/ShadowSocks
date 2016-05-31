namespace SimpleJson.Reflection
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class CacheResolver
    {
        private readonly MemberMapLoader _memberMapLoader;
        private readonly SafeDictionary<Type, SafeDictionary<string, MemberMap>> _memberMapsCache = new SafeDictionary<Type, SafeDictionary<string, MemberMap>>();
        private static readonly SafeDictionary<Type, CtorDelegate> ConstructorCache = new SafeDictionary<Type, CtorDelegate>();

        public CacheResolver(MemberMapLoader memberMapLoader)
        {
            this._memberMapLoader = memberMapLoader;
        }

        private static GetHandler CreateGetHandler(FieldInfo fieldInfo)
        {
            return delegate (object instance) {
                return fieldInfo.GetValue(instance);
            };
        }

        private static GetHandler CreateGetHandler(PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            if (getMethodInfo == null)
            {
                return null;
            }
            return delegate (object instance) {
                return getMethodInfo.Invoke(instance, Type.EmptyTypes);
            };
        }

        private static SetHandler CreateSetHandler(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
            {
                return delegate (object instance, object value) {
                    fieldInfo.SetValue(instance, value);
                };
            }
            return null;
        }

        private static SetHandler CreateSetHandler(PropertyInfo propertyInfo)
        {
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            if (setMethodInfo == null)
            {
                return null;
            }
            return delegate (object instance, object value) {
                setMethodInfo.Invoke(instance, new object[] { value });
            };
        }

        public static object GetNewInstance(Type type)
        {
            CtorDelegate delegate2;
            if (!ConstructorCache.TryGetValue(type, out delegate2))
            {
                ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                delegate2 = delegate {
                    return constructorInfo.Invoke(null);
                };
                ConstructorCache.Add(type, delegate2);
            }
            return delegate2();
        }

        public SafeDictionary<string, MemberMap> LoadMaps(Type type)
        {
            SafeDictionary<string, MemberMap> dictionary;
            if ((type == null) || (type == typeof(object)))
            {
                return null;
            }
            if (!this._memberMapsCache.TryGetValue(type, out dictionary))
            {
                dictionary = new SafeDictionary<string, MemberMap>();
                this._memberMapLoader(type, dictionary);
                this._memberMapsCache.Add(type, dictionary);
            }
            return dictionary;
        }

        private delegate object CtorDelegate();

        public sealed class MemberMap
        {
            public readonly GetHandler Getter;
            public readonly System.Reflection.MemberInfo MemberInfo;
            public readonly SetHandler Setter;
            public readonly System.Type Type;

            public MemberMap(FieldInfo fieldInfo)
            {
                this.MemberInfo = fieldInfo;
                this.Type = fieldInfo.FieldType;
                this.Getter = CacheResolver.CreateGetHandler(fieldInfo);
                this.Setter = CacheResolver.CreateSetHandler(fieldInfo);
            }

            public MemberMap(PropertyInfo propertyInfo)
            {
                this.MemberInfo = propertyInfo;
                this.Type = propertyInfo.PropertyType;
                this.Getter = CacheResolver.CreateGetHandler(propertyInfo);
                this.Setter = CacheResolver.CreateSetHandler(propertyInfo);
            }
        }
    }
}

