using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace NetDoc.Parser.Tests
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public async Task ParseTestProjectRuns()
        {
            var namespacesBegins = new string[]
            {
                            "TestProject"
            }.AsEnumerable();

            var path = @"..\..\..\TestProject\TestProject.csproj";
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projFile = Path.Combine(baseDir, path);
            var docParser = new DocParser();
            await docParser.Parse(projFile, namespacesBegins);
            Assert.IsNotNull(docParser.Data);
            Assert.IsTrue(docParser.Data.Namespaces.Count() > 0);
        }
    }
}
