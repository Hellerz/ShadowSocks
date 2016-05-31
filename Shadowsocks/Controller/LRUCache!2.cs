namespace Shadowsocks.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class LRUCache<K, V> where V: UDPRelay.UDPHandler
    {
        private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap;
        private int capacity;
        private LinkedList<LRUCacheItem<K, V>> lruList;

        public LRUCache(int capacity)
        {
            this.cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
            this.lruList = new LinkedList<LRUCacheItem<K, V>>();
            this.capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(K key, V val)
        {
            if (this.cacheMap.Count >= this.capacity)
            {
                this.RemoveFirst();
            }
            LRUCacheItem<K, V> item = new LRUCacheItem<K, V>(key, val);
            LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(item);
            this.lruList.AddLast(node);
            this.cacheMap.Add(key, node);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V get(K key)
        {
            LinkedListNode<LRUCacheItem<K, V>> node;
            if (this.cacheMap.TryGetValue(key, out node))
            {
                V local = node.Value.value;
                this.lruList.Remove(node);
                this.lruList.AddLast(node);
                return local;
            }
            return default(V);
        }

        private void RemoveFirst()
        {
            LinkedListNode<LRUCacheItem<K, V>> first = this.lruList.First;
            this.lruList.RemoveFirst();
            this.cacheMap.Remove(first.Value.key);
            first.Value.value.Close();
        }
    }
}

