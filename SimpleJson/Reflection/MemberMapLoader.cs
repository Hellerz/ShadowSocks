namespace SimpleJson.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void MemberMapLoader(Type type, SafeDictionary<string, CacheResolver.MemberMap> memberMaps);
}

