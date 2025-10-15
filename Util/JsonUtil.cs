using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Util
{
    public class JsonUtil
    {
        public static string Serialize<T>(T clazz)
        {
            var settings = new JsonSerializerSettings
            {
                // property namse ll be identical to the class property names
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                },
                Formatting = Formatting.Indented
            };


            return JsonConvert.SerializeObject(clazz, settings);
        }

        public static T? Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                }
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

    }
}
