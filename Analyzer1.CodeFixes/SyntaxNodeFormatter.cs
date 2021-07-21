using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using System.Threading;

namespace Analyzer1
{
    public class SyntaxNodeFormatter
    {
        public SyntaxNode FormatNode(SyntaxNode syntaxNode, Document document, CancellationToken cancellationToken = default)
        {
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;

            return Formatter.Format(syntaxNode, originalSolution.Workspace, optionSet, cancellationToken);
        }
    }
}
