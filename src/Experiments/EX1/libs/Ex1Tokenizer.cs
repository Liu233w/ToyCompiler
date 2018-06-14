using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;

namespace Liu233w.Compiler.EX1.Libs
{
    public static class Ex1Tokenizer
    {
        public static Either<List<WrongTokenException>, List<Token>> TokenizeBuffer(string buffer)
        {
            return AutomataTokenizer.GetAllTokenByAutomata(Ex1Automata.ForBegin, buffer)
                .Seprate()
                .Match<Either<List<WrongTokenException>, List<Token>>>(
                    Right: r => r.ExcludeTokenType(Ex1Automata.SpaceToken)
                        .TransformTokenTypeMatched(Ex1Automata.SymbolToken, GetSymbolConvertSet(), TokenTypes.Identifier)
                        .ToList(),
                    Left: l => l
                );
        }

        /// <summary>
        /// 需要进行转换的关键字
        /// </summary>
        private static System.Collections.Generic.HashSet<string> GetSymbolConvertSet() => new System.Collections.Generic.HashSet<string>
        {
            TokenTypes.Thread,
            TokenTypes.Features,
            TokenTypes.Flows,
            TokenTypes.Properties,
            TokenTypes.End,
            TokenTypes.None,
            TokenTypes.In,
            TokenTypes.Out,
            TokenTypes.Data,
            TokenTypes.Port,
            TokenTypes.Event,
            TokenTypes.Parameter,
            TokenTypes.Flow,
            TokenTypes.Source,
            TokenTypes.Sink,
            TokenTypes.Path,
            TokenTypes.Constant,
            TokenTypes.Access,
        };
    }
}