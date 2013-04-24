using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDoc.Parser
{
    public class Startup
    {

        public async Task<object> Invoke(object input)
        {
            var args = (IDictionary<string, object>)input;
            var path = (string)args["path"];

            var data = DocParser.Parse(path);
            
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            string json = await JsonConvert.SerializeObjectAsync(data, Formatting.None, settings);
            return json;
        }

    }
}
