using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer1
{
    internal class MyWriter : CSharpSyntaxRewriter
    {
        private class Node
        {
            public SyntaxNode syntax;
            public ITypeSymbol symbol;
        }

        private readonly Solution originalSolution;
        private readonly IParameterSymbol sourceParameter;
        private readonly ITypeSymbol target;
        private readonly Func<MyWalker> walkerFactory;

        public MyWriter(Solution originalSolution, IParameterSymbol sourceParameter, ITypeSymbol target, SemanticModel semanticModel)
        {
            this.originalSolution = originalSolution;
            this.sourceParameter = sourceParameter;
            this.target = target;
            walkerFactory = () => new MyWalker(semanticModel);
        }


        private static string GetMappingExpression(Func<MyWalker> walkerFactory, Node source, Node target, string sourceParameterName)
        {
            var sourceWalker = walkerFactory();
            sourceWalker.Visit(source.syntax);
            var association = sourceWalker.PropertySymbols.ToDictionary(p => p.symbol.Name);
            var targetWalker = walkerFactory();
            targetWalker.Visit(target.syntax);
            return string.Join(", ", targetWalker.PropertySymbols.Select(pair =>
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