namespace SimpleJson
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class JsonArray : List<object>
    {
        public JsonArray()
        {
        }

        public JsonArray(int capacity) : base(capacity)
        {
        }

        public override string ToString()
        {
            return (SimpleJson.SerializeObject(this) ?? string.Empty);
        }
    }
}

