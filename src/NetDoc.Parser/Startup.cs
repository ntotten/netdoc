namespace NetDoc.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Startup
    {
        public async Task<object> Invoke(object input)
        {
            var args = (IDictionary<string, object>)input;
            var projects = new List<IDictionary<string, string>>();

            ((object[])args["projects"]).Cast<IDictionary<string, object>>()
                          .ToList()
                          .ForEach(
                          p =>
                          {
                              var project = new Dictionary<string, string>();
                              foreach (var projectInfo in p)
                                  project.Add(projectInfo.Key, projectInfo.Value.ToString());
                              
                              projects.Add(project);
                          });

            var namespacesBegins = new string[] 
            {
                "Facebook"
            }.AsEnumerable();

            var docParser = new DocParser();
            docParser.Parse(projects.ToArray(), namespacesBegins);

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            string json = await JsonConvert.SerializeObjectAsync(docParser.Data, Formatting.Indented, settings);
            return json;
        }
    }
}
