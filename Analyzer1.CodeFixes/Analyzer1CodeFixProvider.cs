using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer1
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Analyzer1CodeFixProvider)), Shared]
    public class Analyzer1CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Analyzer1Analyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => MapMethodDeclaration(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private static async Task<Solution> MapMethodDeclaration(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var declaredMethodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

            var target = declaredMethodSymbol.ReturnType;
            var source = declaredMethodSymbol.Parameters.Single();

            ISyntaxWalkerFactory walkerFactory = new SyntaxWalkerFactory(semanticModel);
            ISyntaxRewriter writer = new MySyntaxRewriter(source, target, walkerFactory);

            var updatedMethodDeclaration = writer.Visit(methodDeclaration);

            return await GetUpdatedSolutionAsync(document, methodDeclaration, updatedMethodDeclaration, cancellationToken);
        }

        private static async Task<Solution> GetUpdatedSolutionAsync(Document document, MethodDeclarationSyntax methodDeclaration, SyntaxNode node, CancellationToken cancellationToken)
        {
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var replacedNodeRoot = root.ReplaceNode(methodDeclaration, node);
            var rewrittenRoot = Formatter.Format(replacedNodeRoot, originalSolution.Workspace, optionSet);
            var newDocument = document.WithSyntaxRoot(rewrittenRoot);
            return newDocument.Project.Solution;
        }
    }
}
