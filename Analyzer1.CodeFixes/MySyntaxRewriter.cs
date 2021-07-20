using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Analyzer1
{
    internal class MySyntaxRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
    {
        private class Node
        {
            public SyntaxNode syntax;
            public ITypeSymbol symbol;
        }

        private readonly IParameterSymbol sourceParameter;
        private readonly ITypeSymbol target;
        private readonly ISyntaxWalkerFactory walkerFactory;

        public MySyntaxRewriter(IParameterSymbol sourceParameter, ITypeSymbol target, ISyntaxWalkerFactory walkerFactory)
        {
            this.sourceParameter = sourceParameter;
            this.target = target;
            this.walkerFactory = walkerFactory;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            return base.Visit(node);
        }


        private static string GetMappingExpression(ISyntaxWalkerFactory walkerFactory, Node source, Node target, string sourceParameterName)
        {
            var sourceWalker = walkerFactory.Create();
            sourceWalker.Visit(source.syntax);
            var association = sourceWalker.GetPropertySymbols().ToDictionary(p => p.symbol.Name);
            var targetWalker = walkerFactory.Create();
            targetWalker.Visit(target.syntax);
            return string.Join(", ", targetWalker.GetPropertySymbols().Select(pair =>
            {
                if (pair.walker.HasValue)
                {
                    return ""; // "new {target.Name}";
                }
                else
                {
                    return $"{pair.symbol.Name} = {sourceParameterName}.{association[pair.symbol.Name].symbol.Name}";
                }
            }));
        }

        private static Node FindNode(ITypeSymbol symbol) => new Node { syntax = symbol.DeclaringSyntaxReferences.First().GetSyntax(), symbol = symbol };


        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var association = target.GetMembers().OfType<IFieldSymbol>().ToDictionary(m => m.Name);

            var assignments = string.Join(",", sourceParameter.Type.GetMembers().OfType<IFieldSymbol>()
                .Select(member => $"{association[member.Name].Name} = {sourceParameter.Name}.{member.Name}"));
            var returnStatement = $@"{{
    return new {target.Name}{{{GetMappingExpression(walkerFactory, FindNode(sourceParameter.Type), FindNode(target), sourceParameter.Name)}}};
}}";
            var statement = (BlockSyntax)SyntaxFactory.ParseStatement(returnStatement);
            return node.WithBody(statement);
        }
    }
}