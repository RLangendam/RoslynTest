using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer1
{
    public class SolutionUpdater
    {
        private readonly SyntaxNodeFormatter syntaxNodeFormatter;

        public SolutionUpdater(SyntaxNodeFormatter syntaxNodeFormatter)
        {
            this.syntaxNodeFormatter = syntaxNodeFormatter;
        }
        
        public async Task<Solution> GetUpdatedSolutionAsync(Document document, MethodDeclarationSyntax methodDeclaration, SyntaxNode node, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var rewrittenRoot = root.ReplaceNode(methodDeclaration, node);

            var formattedRoot = syntaxNodeFormatter.FormatNode(rewrittenRoot, document, cancellationToken);

            var newDocument = document.WithSyntaxRoot(formattedRoot);
            return newDocument.Project.Solution;
        }
    }
}
