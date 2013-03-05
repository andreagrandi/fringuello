using System;
using NetServ.Net.Json;
using System.IO;

namespace NTwitter
{
    internal class JsonArray
    {
        #region private fields

        private NetServ.Net.Json.JsonArray innerArray;

        #endregion

        #region private methods

        private static NetServ.Net.Json.JsonArray parseJsonArray(string json)
        {
            JsonParser parser = new JsonParser(new StringReader(json), true);
            NetServ.Net.Json.JsonArray jsonArray = parser.ParseArray();
            parser.Close();
            return jsonArray;
        }

        #endregion

        #region constructor

        public JsonArray(string json)
        {
            try
            {
                innerArray = parseJsonArray(json);
            }
            catch (Exception ex)
            {
                throw new JsonException("JSON cannot be parsed.", ex);
            }
        }

        public JsonArray(NetServ.Net.Json.JsonArray array)
        {
            innerArray = array;
        }

        #endregion

        #region public properties

        public int Count
        {
            get { return innerArray.Count; }
        }

        public JsonObject this[int index]
        {
            get
            {
                NetServ.Net.Json.JsonObject obj = (NetServ.Net.Json.JsonObject)innerArray[index];
                return new JsonObject(obj);
            }
        }

        #endregion

        #region public methods

        public long getLong(int i)
        {
            JsonNumber num = (JsonNumber)innerArray[i];
            return (long)num.Value;
        }

        #endregion
    }
}
