namespace NetDoc.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Startup
    {
        public static async Task<string> ParseProjects(Configuration config)
        {
            var docParser = new DocParser();
            docParser.Parse(config);

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            string json = await JsonConvert.SerializeObjectAsync(docParser.Data, Formatting.Indented, settings);
            return json;
        }

        public async Task<object> Invoke(object input)
        {
            var args = (IDictionary<string, object>)input;

            var projects = new List<ProjectInfo>();

            ((object[])args["projects"]).Cast<IDictionary<string, object>>()
                          .ToList()
                          .ForEach(
                          p =>
                          {
                              var project = new ProjectInfo();
                              project.Path = p["path"] as string;
                              project.Id = p["id"] as string;

                              projects.Add(project);
                          });

            var config = new Configuration()
            {
                Projects = projects,
                FilteredNamespaces = ((object[])args["filteredNamespaces"]).Cast<string>().ToList()
            };

            return await ParseProjects(config);
        }
    }
}
