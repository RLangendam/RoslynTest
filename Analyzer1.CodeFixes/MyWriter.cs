using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Linq;

namespace Analyzer1
{

    internal class MyWriter : CSharpSyntaxRewriter
    { 

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            //var comment = SyntaxFactory.Comment("// Found");
            //node.DescendantNodes().OfType<BlockSyntax>().First().DescendantTrivia().First()
            //node = node.RemoveNode(node.DescendantNodes().OfType<ThrowStatementSyntax>().First(), SyntaxRemoveOptions.KeepExteriorTrivia);
            //return node.WithLeadingTrivia(new[] { comment });


            var statement = (BlockSyntax)SyntaxFactory.ParseStatement(@"{
                                                           // Found
                                                           return new MyModel();
                                                           }");
            return node.WithBody(statement);
            //var newNode = SyntaxFactory.MethodDeclaration(node.ReturnType, node.Identifier);
            //return newNode.AddBodyStatements(statement);
        }        
    }
}