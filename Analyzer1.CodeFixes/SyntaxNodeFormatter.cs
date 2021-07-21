using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using System.Threading;

namespace Analyzer1
{
    public class SyntaxNodeFormatter
    {
        public SyntaxNode FormatNode(SyntaxNode syntaxNode, Document document, CancellationToken cancellationToken = default)
        {
            var workspace = document.Project.Solution.Workspace;
            var optionSet = workspace.Options;

            return Formatter.Format(syntaxNode, workspace, optionSet, cancellationToken);
        }
    }
}
