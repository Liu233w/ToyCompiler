using System;
using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX1.Libs;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Libs
{
    public class Parser
    {
        private readonly IList<Token> _tokens;

        private int _index;

        private LinkedList<ThreadSpec> _parsed;

        public Parser(IList<Token> tokens)
        {
            this._tokens = tokens;
            this._index = 0;
            _parsed = new LinkedList<ThreadSpec>();
        }

        public Application Parse()
        {
            while (HaveNextToken())
            {
                var thread = HandleThread();
                _parsed.AddLast(thread);
            }

            return new Application
            {
                Threads = _parsed,
            };
        }

        private ThreadSpec HandleThread()
        {
            ConsumeType(TokenTypes.Thread);

            var thread = new ThreadSpec
            {
                Identifier = ConsumeType(TokenTypes.Identifier),
                Features = HandleFeatures(),
                Flows = HandleFlows(),
                Properties = HandleProperties(),
            };

            ConsumeType(TokenTypes.End);
            var token = ThisToken();
            EnsureToken(token, TokenTypes.Identifier);
            if (token.Lexeme != thread.Identifier)
            {
                throw new WrongSemanticException("The Identifier in Thread must be matched.");
            }

            return thread;
        }

        #region 便于语法分析的工具函数

        /// <summary>
        /// 确保当前的Token是指定的Type。如果不是，抛出异常；如果是，消耗掉它，并返回词素。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ConsumeType(string type)
        {
            var token = ThisToken();
            EnsureToken(token, type);
            Consume();
            return token.Lexeme;
        }

        /// <summary>
        /// 确保 Token 是特定的类型，否则报错
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        private static void EnsureToken(Token token, string type)
        {
            if (token.TokenType != type)
            {
                throw new TokenNotMatchException($"Expect {type}, but found {token.TokenType}");
            }
        }

        private bool HaveNextToken()
        {
            return _index < _tokens.Count - 1;
        }

        private void Consume()
        {
            ++_index;
        }

        private Token ThisToken()
        {
            try
            {
                return _tokens[_index];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new NotEnoughTokenException("Token不够", e);
            }
        }

        private Token LookAhead(int num)
        {
            try
            {
                return _tokens[_index + num];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new NotEnoughTokenException("Token不够", e);
            }
        }

        #endregion
    }
}