namespace NetDoc.Parser.Tests
{
    using System.Linq;
    using Xunit;

    public class DocParserTests
    {
        [Fact]
        public void ParseTextShouldReturn()
        {
        }

        [Fact]
        public void ParseFilesShouldReturn()
        {
            var namespacesBegins = new string[] 
                {
                    "Facebook"
                }.AsEnumerable();

            var path = @"D:\Github\facebook-csharp-sdk\facebook-csharp-sdk\Source\Facebook\Facebook-Net45.csproj";
            var docParser = new DocParser();
            docParser.Parse(path, namespacesBegins);
            Assert.NotNull(docParser.Data);
            Assert.True(docParser.Data.Namespaces.Count() > 0);
        }
    }
}
