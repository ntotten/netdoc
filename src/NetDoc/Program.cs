namespace NetDoc
{
    using System.IO;
    using NetDoc.Parser;
    using Newtonsoft.Json;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var configurationFile = args[0];
            var config = LoadConfiguration(configurationFile);

            var task = Startup.ParseProjects(config);
            task.Wait();

            System.Console.WriteLine(task.Result);
        }

        private static Configuration LoadConfiguration(string configurationFile)
        {
            Configuration config = default(Configuration);

            using (var reader = File.OpenText(configurationFile))
            {
                var jsonText = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<Configuration>(jsonText);
            }

            return config;
        }
    }
}
