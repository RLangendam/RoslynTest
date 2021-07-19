using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    public static class ITypeSymbolExtensions
    {
        public static TypedConstantType GetTypedConstant(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                case SpecialType.System_Object:
                    return TypedConstantType.Primitive;
                default:
                    switch (type.TypeKind)
                    {
                        case TypeKind.Array:
                            return TypedConstantType.Array;
                        case TypeKind.Enum:
                            return TypedConstantType.Enum;
                        case TypeKind.Error:
                            return TypedConstantType.Error;
                    }

                return TypedConstantType.Type;                                      
            }
        }
    }

    public enum TypedConstantType 
    {
        Error,
        Primitive,
        Enum,
        Type,
        Array
    }

}