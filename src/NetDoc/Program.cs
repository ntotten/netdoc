namespace NetDoc
{
    using System.Collections.Generic;
    using NetDoc.Parser;

    public class Program
    {
        public static void Main(string[] args)
        {
            var input = new Dictionary<string, object>();
            input.Add("path", "F:\\r\\facebooksdk\\forks\\facebook-csharp-sdk\\Source\\Facebook\\Facebook-Net45.csproj");
            
            ////input.Add("path", "F:\\r\\facebooksdk\\forks\\facebook-winclient-sdk\\Source\\Facebook.Client-WindowsStore\\Facebook.Client-WindowsStore.csproj");
            var netDoc = new Startup();
            var task = netDoc.Invoke(input);
            task.Wait();
            var results = task.Result;
        }
    }
}
