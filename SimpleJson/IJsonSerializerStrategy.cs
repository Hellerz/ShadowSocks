namespace SimpleJson
{
    using System;
    using System.Runtime.InteropServices;

    public interface IJsonSerializerStrategy
    {
        object DeserializeObject(object value, Type type);
        bool SerializeNonPrimitiveObject(object input, out object output);
    }
}

