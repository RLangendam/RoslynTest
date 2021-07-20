using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Analyzer1.Test
{
    [TestClass]
    public class MyWalkerTest
    {
        private const string source = @" 
class C
{
    public int IntProperty { get; set; }
    public string StringProperty { get; set; }
    public D DProperty { get; set; }
}

class D
{
    public long LongProperty { get; set; }
}";

        [TestMethod]
        public void SimpleObjectPropertiesFound()
        {
            var tree = CSharpSyntaxTree.ParseText(source);

            var root = tree.GetRoot();

            var cClass = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First();

            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees:new[]  { tree });

            var walker = new MyWalker(compilation.GetSemanticModel(tree));
            walker.Visit(cClass);

            Assert.AreEqual(3, walker.GetPropertySymbols().Count);
        }
    }
}

