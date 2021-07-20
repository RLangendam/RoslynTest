using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    public interface ISyntaxWalkerFactory
    {
        SyntaxWalker CreateAndWalk(SyntaxNode syntax);
    }

    public class SyntaxWalkerFactory : ISyntaxWalkerFactory
    {
        private readonly SemanticModel semanticModel;

        public SyntaxWalkerFactory(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public SyntaxWalker CreateAndWalk(SyntaxNode syntax)
        {
            var walker = new MyWalker(semanticModel);
            walker.Visit(syntax);
            return walker;
        }
    }
}
