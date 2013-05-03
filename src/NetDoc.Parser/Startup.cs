namespace NetDoc.Parser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Startup
    {
        public async Task<object> Invoke(object input)
        {
            var args = (IDictionary<string, object>)input;
            var path = (string)args["path"];

            var data = DocParser.Parse(path);
            
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            string json = await JsonConvert.SerializeObjectAsync(data, Formatting.Indented, settings);
            return json;
        }
    }
}
