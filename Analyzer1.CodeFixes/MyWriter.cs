using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer1
{

    internal class MyWriter : CSharpSyntaxRewriter
    {
        private readonly Solution originalSolution;
        private readonly IParameterSymbol sourceParameter;
        private readonly ITypeSymbol target;

        public MyWriter(Solution originalSolution, IParameterSymbol sourceParameter, ITypeSymbol target)
        {
            this.originalSolution = originalSolution;
            this.sourceParameter = sourceParameter;
            this.target = target;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var association = target.GetMembers().OfType<IFieldSymbol>().ToDictionary(m => m.Name, m => m);
            var assignments = string.Join(",", sourceParameter.Type.GetMembers().OfType<IFieldSymbol>().Select(member => $"{association[member.Name].Name} = {sourceParameter.Name}.{member.Name}"));
            var returnStatement = $@"{{
    return new {target.Name}{{{assignments}}};
}}";
            var statement = (BlockSyntax)SyntaxFactory.ParseStatement(returnStatement);
            return node.WithBody(statement);
        }        
    }
}