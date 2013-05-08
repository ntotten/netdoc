namespace NetDoc
{
    using System.Collections.Generic;
    using NetDoc.Parser;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = new Dictionary<string, object>();

            var project = new Dictionary<string, object>();
            ////project.Add("path", "F:\\r\\facebooksdk\\forks\\facebook-csharp-sdk\\Source\\Facebook\\Facebook-Net45.csproj");
            project.Add("path", "F:\\r\\facebooksdk\\forks\\facebook-winclient-sdk\\Source\\Facebook.Client-WindowsStore\\Facebook.Client-WindowsStore.csproj");

            var projects = new Dictionary<string, object>[] { project };

            input.Add("projects", projects);

            var netDoc = new Startup();
            var task = netDoc.Invoke(input);
            task.Wait();
            var results = task.Result;

            System.Console.WriteLine(results.ToString());
        }
    }
}
