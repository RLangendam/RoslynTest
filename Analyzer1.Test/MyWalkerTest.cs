using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Analyzer1.Test
{
    [TestClass]
    public class MyWalkerTest
    {
        private const string testFilesPath = @".\TestFiles\MyWalker\";

        [TestMethod]
        public void SimpleObjectPropertiesFound()
        {
            var source = GetSource("MyWalkerHappyFlow.cs");

            var tree = CSharpSyntaxTree.ParseText(source);
            
            var cClass = tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().First();
            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees:new[]  { tree });

            var walker = new MyWalker(compilation.GetSemanticModel(tree));
            walker.Visit(cClass);

            Assert.AreEqual(3, walker.GetPropertySymbols().Count);
        }

        private string GetSource(string filename)
        {
            return File.ReadAllText(testFilesPath + filename);
        }
    }
}

