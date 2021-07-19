using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer1
{
    public class MyWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel semanticModel;


        public List<(IPropertySymbol symbol, Optional<MyWalker> walker)> PropertySymbols { get; } = new List<(IPropertySymbol, Optional<MyWalker>)>();

        public MyWalker(SemanticModel semanticModel) : base(SyntaxWalkerDepth.Token)
        {
            this.semanticModel = semanticModel;
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            PropertySymbols.Add(GetWalkedProperty(semanticModel.GetDeclaredSymbol(node)));
            base.VisitPropertyDeclaration(node);
        }

        private (IPropertySymbol, Optional<MyWalker>) GetWalkedProperty(IPropertySymbol propertySymbol)
        {
            var propertyType = propertySymbol.Type;
            var typedConstantType = propertyType.GetTypedConstant();
            if (typedConstantType == TypedConstantType.Primitive)
            {
                return (propertySymbol, new Optional<MyWalker>());
            }
            else if (typedConstantType == TypedConstantType.Type)
            {
                // todo: partial class

                var walker = new MyWalker(semanticModel);
                walker.Visit(propertyType.DeclaringSyntaxReferences.First().GetSyntax());

                return (propertySymbol, walker);

            }
            else
            {
                throw new NotImplementedException("Handle other types, silly ;)");
            }
        }
    }
}