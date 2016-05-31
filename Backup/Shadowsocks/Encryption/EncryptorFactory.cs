namespace Shadowsocks.Encryption
{
    using System;
    using System.Collections.Generic;

    public static class EncryptorFactory
    {
        private static Type[] _constructorTypes = new Type[] { typeof(string), typeof(string) };
        private static Dictionary<string, Type> _registeredEncryptors = new Dictionary<string, Type>();

        static EncryptorFactory()
        {
            foreach (string str in TableEncryptor.SupportedCiphers())
            {
                _registeredEncryptors.Add(str, typeof(TableEncryptor));
            }
            foreach (string str2 in PolarSSLEncryptor.SupportedCiphers())
            {
                _registeredEncryptors.Add(str2, typeof(PolarSSLEncryptor));
            }
            foreach (string str3 in SodiumEncryptor.SupportedCiphers())
            {
                _registeredEncryptors.Add(str3, typeof(SodiumEncryptor));
            }
        }

        public static IEncryptor GetEncryptor(string method, string password)
        {
            if (string.IsNullOrEmpty(method))
            {
                method = "table";
            }
            method = method.ToLowerInvariant();
            Type type = _registeredEncryptors[method];
            return (IEncryptor) type.GetConstructor(_constructorTypes).Invoke(new object[] { method, password });
        }
    }
}

