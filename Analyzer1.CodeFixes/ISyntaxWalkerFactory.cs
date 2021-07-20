using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    public interface ISyntaxWalkerFactory
    {
        SyntaxWalker Create();
    }

    public class SyntaxWalkerFactory : ISyntaxWalkerFactory
    {
        private readonly SemanticModel semanticModel;

        public SyntaxWalkerFactory(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public SyntaxWalker Create()
        {
            return new MyWalker(semanticModel);
        }
    }
}
