using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Analyzer1
{
    public abstract class SyntaxWalker : CSharpSyntaxWalker
    {
        public abstract List<(IPropertySymbol symbol, Optional<MyWalker> walker)> GetPropertySymbols();

        public SyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
        {
        }
    }
}