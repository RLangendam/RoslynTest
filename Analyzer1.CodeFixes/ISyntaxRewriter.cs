using Microsoft.CodeAnalysis;

namespace Analyzer1
{
    internal interface ISyntaxRewriter
    {
        SyntaxNode Visit(SyntaxNode node);
    }
}