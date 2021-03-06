﻿namespace SimpleJson
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        private readonly Dictionary<string, object> _members = new Dictionary<string, object>();

        public void Add(KeyValuePair<string, object> item)
        {
            this._members.Add(item.Key, item.Value);
        }

        public void Add(string key, object value)
        {
            this._members.Add(key, value);
        }

        public void Clear()
        {
            this._members.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return (this._members.ContainsKey(item.Key) && (this._members[item.Key] == item.Value));
        }

        public bool ContainsKey(string key)
        {
            return this._members.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            int count = this.Count;
            foreach (KeyValuePair<string, object> pair in this)
            {
                array[arrayIndex++] = pair;
                if (--count <= 0)
                {
                    break;
                }
            }
        }

        internal static object GetAtIndex(IDictionary<string, object> obj, int index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (index >= obj.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int num = 0;
            foreach (KeyValuePair<string, object> pair in obj)
            {
                if (num++ == index)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this._members.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return this._members.Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        public override string ToString()
        {
            return SimpleJson.SimpleJson.SerializeObject(this);
        }

        public bool TryGetValue(string key, out object value)
        {
            return this._members.TryGetValue(key, out value);
        }

        public int Count
        {
            get
            {
                return this._members.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[string key]
        {
            get
            {
                return this._members[key];
            }
            set
            {
                this._members[key] = value;
            }
        }

        public object this[int index]
        {
            get
            {
                return GetAtIndex(this._members, index);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this._members.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return this._members.Values;
            }
        }
    }
}

