using System;
using System.Collections.Generic;
using System.Linq;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX1.Libs;
using Liu233w.Compiler.EX2.Exceptions;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Libs
{
    public class Parser
    {
        private readonly IList<Token> _tokens;

        private int _index;

        private readonly LinkedList<ThreadSpec> _parsed;

        private readonly LinkedList<ParseException> _exceptions;

        public Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _index = 0;
            _parsed = new LinkedList<ThreadSpec>();
            _exceptions = new LinkedList<ParseException>();
        }

        public (Application, ICollection<ParseException>) Parse()
        {
            var application = new Application
            {
                BeginPosition = _tokens.First().TokenBeginIdx,
                EndPosition = _tokens.Last().TokenEndIdx,
                Threads = _parsed
            };

            try
            {
                while (HaveNextToken())
                {
                    var thread = HandleThread();
                    _parsed.AddLast(thread);
                }
            }
            catch (NotEnoughTokenException e)
            {
            }

            return (application, _exceptions);
        }

        private ThreadSpec HandleThread()
        {
            var thread = new ThreadSpec
            {
                BeginPosition = ThisToken().TokenBeginIdx
            };
            TryParseAndResumeToToken(() =>
            {
                ConsumeType(TokenTypes.Thread);

                thread.Identifier = ConsumeType(TokenTypes.Identifier);

                var resumableType = new[]
                {
                    TokenTypes.Features,
                    TokenTypes.Flows,
                    TokenTypes.Properties,
                    TokenTypes.End,
                };
                TryParseAndResumeToToken(() => thread.Features = HandleFeatures(), resumableType);
                TryParseAndResumeToToken(() => thread.Flows = HandleFlows(), resumableType);
                TryParseAndResumeToToken(() => thread.Properties = HandleProperties(), resumableType);

                ConsumeType(TokenTypes.End);
                var token = ThisToken();
                EnsureToken(token, TokenTypes.Identifier);
                if (token.Lexeme != thread.Identifier.Lexeme)
                {
                    throw Error(new SemanticException("The Identifier in Thread must be matched.", token, thread));
                }

                ConsumeType(TokenTypes.Semicolon);
            }, "thread");

            thread.EndPosition = LookAhead(-1).TokenEndIdx;
            return thread;
        }

        private FeatureSpec HandleFeatures()
        {
            if (ThisToken().TokenType != TokenTypes.Features)
            {
                return new NoneFeature
                {
                    BeginPosition = ThisToken().TokenBeginIdx,
                    EndPosition = ThisToken().TokenEndIdx
                };
            }

            Consume();

            // IOType 可能有两个 Token，所以要考虑这两种情况
            if (LookAhead(3).Match(TokenTypes.Parameter) || LookAhead(4).Match(TokenTypes.Parameter))
            {
                return HandleParameterSpec();
            }
            else
            {
                return HandlePortSpec();
            }
        }

        private ParameterSpec HandleParameterSpec()
        {
            // parameterSpec
            var spec = new ParameterSpec
            {
                Identifier = ThisToken(),
                BeginPosition = ThisToken().TokenBeginIdx
            };

            ConsumeType(TokenTypes.SingleColon);

            switch (ThisToken().TokenType)
            {
                case TokenTypes.Out:
                {
                    Consume();
                    spec.IoType = IoType.Out;
                    break;
                }
                case TokenTypes.In:
                {
                    Consume();
                    if (ThisToken().Match(TokenTypes.Out))
                    {
                        Consume();
                        spec.IoType = IoType.InOut;
                    }
                    else
                    {
                        spec.IoType = IoType.In;
                    }

                    break;
                }
                default:
                    throw Error(new GrammarException("Expect in/out/in out", ThisToken(), spec));
            }

            ConsumeType(TokenTypes.Parameter);

            if (!ThisToken().Match(TokenTypes.LeftBrace, TokenTypes.Semicolon))
            {
                spec.Reference = HandleReference();
            }
            else
            {
                spec.Reference = new NoneReference();
            }

            if (ThisToken().Match(TokenTypes.LeftBrace))
            {
                spec.Associations = HandleAssociationBlock();
            }
            else
            {
                spec.Associations = new AssociationBlock
                {
                    Associations = new AssociationBase[] { },
                };
            }
        }

        private Reference HandleReference()
        {
            if (LookAhead(1).Match(TokenTypes.DoubleColon))
            {
                HandlePackage
            }
        }

        #region 便于语法分析的工具函数

        /// <summary>
        /// 执行委托，如果抛出异常，则尝试从 resumeableType 处恢复。
        /// </summary>
        /// <returns>如果正确执行，返回true；否则返回false</returns>
        private bool TryParseAndResumeToToken(Action action, params string[] resumeableTypes)
        {
            try
            {
                action();
                return true;
            }
            catch (ParseException e)
            {
                if (e is NotEnoughTokenException)
                {
                    throw;
                }

                while (true)
                {
                    foreach (var type in resumeableTypes)
                    {
                        if (ThisToken().Match(type))
                        {
                            return false;
                        }
                    }

                    Consume();
                }
            }
        }

        /// <summary>
        /// 记录异常，并再次返回这个异常
        /// </summary>
        private ParseException Error(ParseException e)
        {
            return _exceptions.AddLast(e).Value;
        }

        /// <summary>
        /// 确保当前的Token是指定的Type。如果不是，抛出异常；如果是，消耗掉它，并返回Token
        /// </summary>
        private Token ConsumeType(string type)
        {
            var token = ThisToken();
            EnsureToken(token, type);
            Consume();
            return token;
        }

        /// <summary>
        /// 确保 Token 是特定的类型，否则报错
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        private void EnsureToken(Token token, string type)
        {
            if (!token.Match(type))
            {
                throw Error(new GrammarException($"Expect {type}, but found {token.TokenType}", token, null));
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
                throw Error(new NotEnoughTokenException("Token不够", null, null));
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
                throw Error(new NotEnoughTokenException("Token不够", null, null));
            }
        }

        #endregion
    }
}