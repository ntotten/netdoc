using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetDoc.Parser.Tests
{
    public class DocParserTests
    {

        [Fact]
        public void ParseTextShouldReturn()
        {

        }

        [Fact]
        public void ParseFilesShouldReturn()
        {
            var path = @"C:\Users\Nathan\Github\facebook-csharp-sdk\facebook-csharp-sdk\Source\Facebook";
            var parser = new DocParser();
            var result = parser.Parse(path);
            Assert.NotNull(result);
        }

    }
}
