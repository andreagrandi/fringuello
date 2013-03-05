using System;
using System.IO;
using NetServ.Net.Json;

namespace NTwitter
{
    internal class JsonObject
    {
        #region private fields

        private NetServ.Net.Json.JsonObject innerObject;

        #endregion

        #region private methods

        private static NetServ.Net.Json.JsonObject parseJsonObject(string basetext)
        {
            JsonParser parser = new JsonParser(new StringReader(basetext), true);
            NetServ.Net.Json.JsonObject jsonObject = parser.ParseObject();
            parser.Close();
            return jsonObject;
        }

        #endregion

        #region constructor

        internal JsonObject(NetServ.Net.Json.JsonObject obj)
        {
            this.innerObject = obj;
        }

        public JsonObject(string json)
        {
            try
            {
                innerObject = parseJsonObject(json);
            }
            catch (Exception ex)
            {
                throw new JsonException("JSON cannot be parsed.", ex);
            }
        }

        #endregion

        #region public methods

        public long getLong(string key)
        {
            JsonNumber num = (JsonNumber)innerObject[key];
            return (long)num.Value;
        }

        public string getString(string key)
        {
            IJsonType type = innerObject[key];
            if (type.JsonTypeCode == JsonTypeCode.String)
            {
                JsonString txt = (JsonString)innerObject[key];
                return txt.Value;
            }
            else
            {
                return null;
            }
        }

        public bool getbool(string key)
        {
            JsonBoolean bl = (JsonBoolean)innerObject[key];
            return bl.Value;
        }

        public JsonArray getJSONArray(string key)
        {
            NetServ.Net.Json.JsonArray arr = (NetServ.Net.Json.JsonArray)innerObject[key];
            return new JsonArray(arr);
        }

        public JsonObject getJSONObject(string key)
        {
            NetServ.Net.Json.JsonObject obj = (NetServ.Net.Json.JsonObject)innerObject[key];
            return new JsonObject(obj);
        }

        public JsonObject optJSONObject(string key)
        {
            if (innerObject.ContainsKey(key))
            {
                return this.getJSONObject(key);
            }
            else
            {
                return null;
            }
        }

        public bool has(string key)
        {
            return innerObject.ContainsKey(key);
        }

        public int getInt(string key)
        {
            JsonNumber num = (JsonNumber)innerObject[key];
            return (int)num.Value;
        }

        #endregion
    }
}
