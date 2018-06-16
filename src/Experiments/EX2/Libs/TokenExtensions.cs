using System.Linq;
using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Libs
{
    /// <summary>
    /// Token 的扩展方法，便于写语法分析
    /// </summary>
    public static class TokenExtensions
    {
        public static bool Match(this Token token, string type)
        {
            return token.TokenType == type;
        }

        public static bool MatchAny(this Token token, params string[] types)
        {
            return types.Any(type => token.TokenType == type);
        }
    }
}