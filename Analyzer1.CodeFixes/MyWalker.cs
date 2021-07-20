using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer1
{

    public class MyWalker : SyntaxWalker
    {
        private readonly SemanticModel semanticModel;
        private readonly List<(IPropertySymbol symbol, Optional<MyWalker> walker)> propertySymbols = new List<(IPropertySymbol, Optional<MyWalker>)>();

        public override List<(IPropertySymbol symbol, Optional<MyWalker> walker)> GetPropertySymbols()
        {
            return propertySymbols;
        }

        public MyWalker(SemanticModel semanticModel) : base(SyntaxWalkerDepth.Token)
        {
            this.semanticModel = semanticModel;
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            GetPropertySymbols().Add(GetWalkedProperty(semanticModel.GetDeclaredSymbol(node)));
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