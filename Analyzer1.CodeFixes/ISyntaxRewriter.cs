using Microsoft.CodeAnalysis;

namespace Analyzer1
{
    public interface ISyntaxRewriter
    {
        SyntaxNode Visit();
    }
}