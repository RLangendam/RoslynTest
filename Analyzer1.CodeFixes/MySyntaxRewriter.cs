using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

        SyntaxNode ISyntaxRewriter.Visit(SyntaxNode node)
        {
            return base.Visit(node);
        }


        private static string GetMappingExpression(List<(IPropertySymbol symbol, Optional<MyWalker> walker)> sourcePropertySymbols, 
                                                   List<(IPropertySymbol symbol, Optional<MyWalker> walker)> targetPropertySymbols, 
                                                   string sourcePropertyStem)
        {
            var association = sourcePropertySymbols.ToDictionary(p => p.symbol.Name);

            return string.Join(", ", targetPropertySymbols.Select(pair =>
            {
                if (pair.walker.HasValue)
                {
                    return $@"{pair.symbol.Name} = new {pair.symbol.Type}{{ {
                        GetMappingExpression(association[pair.symbol.Name].walker.Value.GetPropertySymbols(), 
                                             pair.walker.Value.GetPropertySymbols(),
                                             $"{sourcePropertyStem}.{pair.symbol.Name}")
                        } }}";
                }
                else
                {
                    return $"{pair.symbol.Name} = {sourcePropertyStem}.{association[pair.symbol.Name].symbol.Name}";
                }
            }));
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Node FindNode(ITypeSymbol symbol) => new Node { syntax = symbol.DeclaringSyntaxReferences.First().GetSyntax(), symbol = symbol };

            var sourceWalker = walkerFactory.CreateAndWalk(FindNode(sourceParameter.Type).syntax);
            var targetWalker = walkerFactory.CreateAndWalk(FindNode(target).syntax);
            var returnStatement = $@"{{
    return new {target.Name}{{{
                GetMappingExpression(sourceWalker.GetPropertySymbols(), targetWalker.GetPropertySymbols(), sourceParameter.Name)
                }}};
}}";
            var statement = (BlockSyntax)SyntaxFactory.ParseStatement(returnStatement);
            return node.WithBody(statement);
        }
    }
}