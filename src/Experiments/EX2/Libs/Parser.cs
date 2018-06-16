﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly Dictionary<string, int> _resumeTable;

        public Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _index = 0;
            _parsed = new LinkedList<ThreadSpec>();
            _exceptions = new LinkedList<ParseException>();
            _resumeTable = new Dictionary<string, int>();
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

        private PortSpec HandlePortSpec()
        {
            var portSpec = new PortSpec
            {
                BeginPosition = ThisToken().TokenBeginIdx,
            };

            TryParseAndResumeToToken(() =>
            {
                TryParseAndResumeToToken(() =>
                {
                    // Identifier
                    portSpec.Identifier = ConsumeType(TokenTypes.Identifier);
                }, TokenTypes.SingleColon);

                TryParseAndResumeToToken(() =>
                {
                    ConsumeType(TokenTypes.SingleColon);
                    portSpec.IoType = HandleIoType();
                }, TokenTypes.Data, TokenTypes.Event);

                portSpec.PortType = HandlePortType();

                if (ThisToken().Match(TokenTypes.LeftBrace))
                {
                    portSpec.Associations = HandleAssociationBlock();
                }
                else
                {
                    portSpec.Associations = new AssociationBlock
                    {
                        Associations = new AssociationBase[] { },
                    };
                }
            }, TokenTypes.Semicolon);
        }

        private PortType HandlePortType()
        {
            switch (ThisToken().TokenType)
            {
                case TokenTypes.Data:
                {
                    var dataPort = new DataPort
                    {
                        BeginPosition = ThisToken().TokenBeginIdx
                    };
                    Consume();
                    ConsumeType(TokenTypes.Port);

                    dataPort.Reference = TryHandleReferenceOrNone();
                    dataPort.EndPosition = dataPort.Reference.EndPosition;

                    return dataPort;
                }
                case TokenTypes.Event:
                {
                    var beginPosition = ThisToken().TokenBeginIdx;
                    Consume();

                    if (ThisToken().Match(TokenTypes.Data))
                    {
                        Consume();
                        var eventDataPort = new EventDataPort
                        {
                            BeginPosition = beginPosition
                        };
                        ConsumeType(TokenTypes.Port);

                        eventDataPort.Reference = TryHandleReferenceOrNone();
                        eventDataPort.EndPosition = eventDataPort.Reference.EndPosition;

                        return eventDataPort;
                    }
                    else if (ThisToken().Match(TokenTypes.Port))
                    {
                        var eventPort = new EventPort
                        {
                            BeginPosition = beginPosition,
                            EndPosition = ThisToken().TokenEndIdx
                        };
                        Consume();
                        return eventPort;
                    }
                    else
                    {
                        goto default;
                    }
                }
                default:
                    throw Error(new GrammarException("Expect 'data port', 'event data port' or '"))
            }
        }

        private ReferenceBase TryHandleReferenceOrNone()
        {
            if (ThisToken().Match(TokenTypes.Identifier))
            {
                return HandleReference();
            }
            else
            {
                return new NoneReference
                {
                    BeginPosition = ThisToken().TokenBeginIdx,
                    EndPosition = ThisToken().TokenEndIdx
                };
            }
        }

        private IoType HandleIoType()
        {
            switch (ThisToken().TokenType)
            {
                case TokenTypes.Out:
                {
                    Consume();
                    return IoType.Out;
                }
                case TokenTypes.In:
                {
                    Consume();
                    if (ThisToken().Match(TokenTypes.Out))
                    {
                        Consume();
                        return IoType.InOut;
                    }
                    else
                    {
                        return IoType.In;
                    }
                }
                default:
                    throw Error(new GrammarException("Expect in/out/in out", ThisToken(), spec));
            }
        }

        private ParameterSpec HandleParameterSpec()
        {
            var spec = new ParameterSpec
            {
                BeginPosition = ThisToken().TokenBeginIdx,
            };

            TryParseAndResumeToToken(() =>
            {
                TryParseAndResumeToToken(() =>
                {
                    // identifier
                    spec.Identifier = ConsumeType(TokenTypes.Identifier);
                }, TokenTypes.SingleColon);

                TryParseAndResumeToToken(() =>
                {
                    ConsumeType(TokenTypes.SingleColon);
                    spec.IoType = HandleIoType();
                }, TokenTypes.Parameter);

                ConsumeType(TokenTypes.Parameter);

                spec.Reference = TryHandleReferenceOrNone();

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
            }, TokenTypes.Semicolon);

            var token = ConsumeType(TokenTypes.Semicolon);
            spec.EndPosition = token.TokenEndIdx;

            return spec;
        }

        private AssociationBlock HandleAssociationBlock()
        {
            var associations = new LinkedList<AssociationBase>();
            var associationBlock = new AssociationBlock
            {
                Associations = associations,
                BeginPosition = ThisToken().TokenBeginIdx,
            };
            ConsumeType(TokenTypes.LeftBrace);

            TryParseAndResumeToToken(() =>
            {
                if (!ThisToken().Match(TokenTypes.RightBrace))
                {
                    associations.AddLast(HandleAssociation());
                }
            }, TokenTypes.RightBrace);

            associationBlock.EndPosition = ThisToken().TokenEndIdx;
            ConsumeType(TokenTypes.RightBrace);

            return associationBlock;
        }

        private AssociationBase HandleAssociation()
        {
            if (ThisToken().Match(TokenTypes.None))
            {
                var noneToken = ConsumeType(TokenTypes.None);
                return new NoneAssociation
                {
                    BeginPosition = noneToken.TokenBeginIdx,
                    EndPosition = noneToken.TokenEndIdx,
                };
            }

            var association = new Association
            {
                BeginPosition = ThisToken().TokenBeginIdx,
            };

            TryParseAndResumeToToken(() =>
            {
                if (LookAhead(1).Match(TokenTypes.DoubleColon))
                {
                    association.PreIdentifier = ConsumeType(TokenTypes.Identifier);
                    Consume();
                }

                association.Identifier = ConsumeType(TokenTypes.Identifier);
                if (ThisToken().Match(TokenTypes.Arraw1))
                {
                    association.Splitter = Splitter.Arrow;
                }
                else if (ThisToken().Match(TokenTypes.Arraw2))
                {
                    association.Splitter = Splitter.PlusArrow;
                }
                else
                {
                    throw Error(new GrammarException("Expect => or +=>", ThisToken(), association));
                }

                if (ThisToken().Match(TokenTypes.Constant))
                {
                    association.Constant = true;
                    Consume();
                }
                else
                {
                    association.Constant = false;
                }
            }, TokenTypes.Access);

            ConsumeType(TokenTypes.Access);

            association.Decimal = ConsumeType(TokenTypes.Decimal);

            return association;
        }

        private Reference HandleReference()
        {
            var reference = new Reference
            {
                BeginPosition = ThisToken().TokenBeginIdx,
            };

            if (LookAhead(1).Match(TokenTypes.DoubleColon))
            {
                reference.PackageName = HandlePackageName();
            }
            else
            {
                reference.PackageName = new NonePackageName();
            }

            var identifier = ConsumeType(TokenTypes.Identifier);
            reference.EndPosition = identifier.TokenEndIdx;
            reference.Identifier = identifier;

            return reference;
        }

        private PackageName HandlePackageName()
        {
            var nameList = new LinkedList<Token>();
            var packageName = new PackageName
            {
                BeginPosition = ThisToken().TokenBeginIdx,
                Identifiers = nameList,
            };

            Token lastColon = null;
            while (LookAhead(1).Match(TokenTypes.DoubleColon))
            {
                nameList.AddLast(ConsumeType(TokenTypes.Identifier));
                lastColon = ConsumeType(TokenTypes.DoubleColon);
            }

            Debug.Assert(lastColon != null, nameof(lastColon) + " != null, 在调用时确保了至少有一个 ::");
            packageName.EndPosition = lastColon.TokenEndIdx;
            return packageName;
        }

        #region 便于语法分析的工具函数

        /// <summary>
        /// 执行委托，如果抛出异常，则尝试从 resumeableType 处恢复。
        /// 这个有可能会嵌套，所以先将能够恢复的Token放到一个表中，假如找到了一个元素，说明可能在上一级，
        /// </summary>
        /// <returns>如果正确执行，返回true；否则返回false</returns>
        private bool TryParseAndResumeToToken(Action action, params string[] resumeableTypes)
        {
            foreach (var type in resumeableTypes)
            {
                ++_resumeTable[type];
            }

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

                var typeSet = resumeableTypes.ToHashSet();

                while (true)
                {
                    var tokenType = ThisToken().TokenType;
                    if (typeSet.Contains(tokenType))
                    {
                        // 就是这个函数中标注的 token
                        return false;
                    }
                    else if (_resumeTable[tokenType] > 0)
                    {
                        // 之前的函数中有这个，应当恢复到之前的状态
                        throw;
                    }

                    Consume();
                }
            }
            finally
            {
                foreach (var type in resumeableTypes)
                {
                    --_resumeTable[type];
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