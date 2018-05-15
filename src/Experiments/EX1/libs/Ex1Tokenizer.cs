using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;

namespace Liu233w.Compiler.EX1.libs
{
    public static class Ex1Tokenizer
    {
        public static Either<List<WrongTokenException>, List<Token>> TokenizeBuffer(string buffer)
        {
            return AutomataTokenizer.GetAllTokenByAutomata(Ex1Automata.ForBegin, buffer)
                .Seprate()
                .Match<Either<List<WrongTokenException>, List<Token>>>(
                    Right: r => ConvertSymbolToIdentifiers(r.ExcludeTokenType(Ex1Automata.SpaceToken)).ToList(),
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

        /// <summary>
        /// 将标记为 Symbol 的语法单元转换成相应的关键词或 Identifier
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static IEnumerable<Token> ConvertSymbolToIdentifiers(IEnumerable<Token> tokens)
        {
            var set = GetSymbolConvertSet();
            foreach (var token in tokens)
            {
                if (token.TokenType == Ex1Automata.SymbolToken)
                {
                    token.TokenType = set.Contains(token.Lexem) ? token.Lexem : TokenTypes.Identifier;
                }
                yield return token;
            }
        }
    }
}