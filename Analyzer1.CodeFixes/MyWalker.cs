using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    internal class MyWalker : CSharpSyntaxWalker
    {
        public MyWalker() : base(SyntaxWalkerDepth.Token) { }


    }
}