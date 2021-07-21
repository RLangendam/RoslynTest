using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Analyzer1.Test
{
    [TestClass]
    public class MySyntaxRewriterTest
    {
        private const string testFilesPath = @".\TestFiles\MySyntaxRewriter\";

        [DataTestMethod]
        [DataRow("BasicPropertiesStart.cs", "BasicPropertiesFix.cs")]
        [DataRow("RecursivePropertiesStart.cs", "RecursivePropertiesFix.cs")]
        [DataRow("TargetPropertiesSubsetOfSourcePropertiesStart.cs", "TargetPropertiesSubsetOfSourcePropertiesFix.cs")]
        [DataRow("SourcePropertiesSubsetOfTargetPropertiesStart.cs", "SourcePropertiesSubsetOfTargetPropertiesFix.cs")]
        // todo: [DataRow("SourcePropertiesSubsetOfTargetPropertiesStart2.cs", "SourcePropertiesSubsetOfTargetPropertiesFix2.cs")]
        public async Task MappingBasicPropertiesTestAsync(string startFilename, string fixFilename)
        {
            var start = GetFileAsString(startFilename);
            var fix = GetFileAsString(fixFilename);

            var document = CreateDocument(start);
            var rewriter = await GetMySyntaxRewriterAsync(document);

            var newNode = rewriter.Visit();

            var result = FormatNode(newNode, document);

            Assert.AreEqual(fix, result); // todo: check if we can use funky diff tool here like in old test verify method.
        }

        private string GetFileAsString(string filename)
        {
            return File.ReadAllText(testFilesPath + filename);
        }

        private string FormatNode(SyntaxNode node, Document document)
        {
            var formatter = new SyntaxNodeFormatter();

            return formatter.FormatNode(node, document).ToFullString();
        }

        private async Task<ISyntaxRewriter> GetMySyntaxRewriterAsync(Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var declaration = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

            var factory = new SyntaxRewriterFactory(document, declaration);
            return await factory.GetMySyntaxRewriterAsync();
        }

        private Document CreateDocument(string sourceText)
        {
            var workspace = new AdhocWorkspace();
            string projectName = "TestProject";
            ProjectId projectId = ProjectId.CreateNewId();
            VersionStamp versionStamp = VersionStamp.Create();
            ProjectInfo helloWorldProject = ProjectInfo.Create(projectId, versionStamp, projectName, projectName, LanguageNames.CSharp);
            SourceText documentText = SourceText.From(sourceText);

            Project newProject = workspace.AddProject(helloWorldProject);
            return workspace.AddDocument(newProject.Id, "Source.cs", documentText);
        }
    }
}
