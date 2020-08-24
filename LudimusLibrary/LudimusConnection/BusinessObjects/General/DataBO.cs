using Newtonsoft.Json;

namespace LudimusConnection.BusinessObjects.General
{
    public class DataBO<T>
    {
        public string Key { get; set; }

        public T Value { get; set; }

        internal string GetValueAsJson()
        {
            return JsonConvert.SerializeObject(Value);
        }
    }
}