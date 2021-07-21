using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            // todo: Some form of DI??
            var syntaxRewriterFactory = new SyntaxRewriterFactory(document, methodDeclaration, cancellationToken);
            var syntaxNodeFormatter = new SyntaxNodeFormatter();
            var solutionUpdater = new SolutionUpdater(syntaxNodeFormatter);

            var writer = await syntaxRewriterFactory.GetMySyntaxRewriterAsync();

            var updatedMethodDeclaration = writer.Visit();

            return await solutionUpdater.GetUpdatedSolutionAsync(document, methodDeclaration, updatedMethodDeclaration, cancellationToken);            
        }
    }
}
