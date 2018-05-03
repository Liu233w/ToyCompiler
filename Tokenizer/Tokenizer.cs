using System;
using System.Collections.Generic;

namespace Tokenizer
{
    public class Tokenizer
    {
        public AutomataState Automata { get; set; }

        public Tokenizer()
        {
            var nameEndState = new AutomataState
            {
                Asserter = char.IsLetterOrDigit,
                IdentifierType = "name",
            };
            nameEndState.Next.Add(nameEndState);

            var commentBody = new AutomataState
            {
                Asserter = c => true,
                Next =
                {
                    new AutomataState
                    {
                        Asserter = c => c == '*',
                        Next =
                        {
                            new AutomataState
                            {
                                Asserter = c => c == '/',
                                IdentifierType = "comment",
                            }
                        }
                    },
                    // commentBody,
                }
            };
            commentBody.Next.Add(commentBody);

            var space = new AutomataState
            {
                Asserter = c => c == ' ' || c == '\n' || c == '\r',
                IdentifierType = "space",
            };
            space.Next.Add(space);

            var integer = new AutomataState
            {
                Asserter = char.IsDigit,
                IdentifierType = "integer",
            };
            integer.Next.Add(integer);

            Automata = new AutomataState
            {
                Asserter = null,
                IdentifierType = null,
                Next =
                {
                    space,
                    new AutomataState
                    {
                        Asserter = c => c == ';',
                        IdentifierType = ";",
                    },
                    integer,
                    new AutomataState
                    {
                        Asserter = char.IsLetter,
                        IdentifierType = null,
                        Next =
                        {
                            nameEndState,
                        }
                    },
                    new AutomataState
                    {
                        Asserter = c => c == '/',
                        IdentifierType = null,
                        Next =
                        {
                            new AutomataState
                            {
                                Asserter = c => c == '*',
                                IdentifierType = null,
                                Next = {commentBody}
                            }
                        }
                    }
                }
            };
        }

        public IEnumerable<Identifier> Tokenize(string buffer)
        {
            int start = 0;
            while (start < buffer.Length)
            {
                var identifier = AutomataTokenizer.GetByAutomata(Automata, buffer, start);
                if (identifier.Recognized == null)
                {
                    throw new InvalidOperationException("出现语法错误");
                }
                yield return identifier;
                start = identifier.LastPosition;
            }

            yield break;
        }
    }
}