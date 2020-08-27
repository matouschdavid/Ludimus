
using Newtonsoft.Json;
using System;

namespace LudimusConnection.BusinessObjects.General
{
    public class DataBO<T>
    {
        public string Key;

        public T Value;

        public Type Type;

        public string LeftOver;
        internal string GetValueAsJson()
        {
            return JsonConvert.SerializeObject(Value);
        }

        internal string GetAsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public DataBO()
        {

        }

        public DataBO(string key, T value, Type type)
        {
            Key = key;
            Value = value;
            Type = type;
        }

        public DataBO(string key, T value)
        {
            Key = key;
            Value = value;
            Type = typeof(T);
        }

        public DataBO(DataBO<T> data)
        {
            this.Key = data.Key;
            this.Value = data.Value;
            this.Type = data.Type;
        }

        public DataBO(string json)
        {
            if (json == String.Empty)
            {
                LeftOver = "";
                return;
            }
                
            try
            {
                string lenStr = json.Split('|')[0];
                var tmp = lenStr.Split('}');

                int len = Convert.ToInt32(tmp.Length > 1 ? tmp[tmp.Length - 1] : lenStr);

                LeftOver = json.Substring(lenStr.Length + len + 1);
                json = json.Substring(lenStr.Length + 1, len);
                var obj = JsonConvert.DeserializeObject<DataBO<T>>(json);
                Key = obj.Key;
                Value = obj.Value;
                Type = obj.Type;
            }
            catch (Exception)
            {
                LeftOver = "";
            }
        }

        public override string ToString()
        {
            return this.Key + " : " + this.Value;
        }
    }
}