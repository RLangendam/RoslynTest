using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer1
{
    public class SyntaxRewriterFactory
    {
        private readonly Document document;
        private readonly MethodDeclarationSyntax methodDeclaration;
        private readonly CancellationToken cancellationToken;
        
        public SyntaxRewriterFactory(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken=default)
        {
            this.document = document;
            this.methodDeclaration = methodDeclaration;
            this.cancellationToken = cancellationToken;
        }

        public async Task<ISyntaxRewriter> GetMySyntaxRewriterAsync()
        {
            var paramters = await GetWriterParamtersAsync();
            return new MySyntaxRewriter(paramters.source, paramters.target, paramters.walkerFactory, methodDeclaration);
        }
     
        private async Task<(IParameterSymbol source, ITypeSymbol target, ISyntaxWalkerFactory walkerFactory)> GetWriterParamtersAsync()
        {
            var methodSymbol = await GetDeclaredMethodSymbolAsync();
            return (methodSymbol.Parameters.Single(), methodSymbol.ReturnType, new SyntaxWalkerFactory(await GetSemanticModelAsync()));
        }

        private async Task<IMethodSymbol> GetDeclaredMethodSymbolAsync()
        {
            var semanticModel = await GetSemanticModelAsync();
            return semanticModel.GetDeclaredSymbol(methodDeclaration);
        }

        private async Task<SemanticModel> GetSemanticModelAsync() => await document.GetSemanticModelAsync(cancellationToken);
    }
}
