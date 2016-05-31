namespace Shadowsocks.Controller
{
    using System;

    internal class LRUCacheItem<K, V>
    {
        public K key;
        public V value;

        public LRUCacheItem(K k, V v)
        {
            this.key = k;
            this.value = v;
        }
    }
}

