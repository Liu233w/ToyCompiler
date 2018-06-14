using System;
using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Libs
{
    public class Parser
    {
        private readonly IList<Token> _tokens;

        private int _index;

        private LinkedList<ThreadSpec> _parsed;

        private ThreadSpec _now = null;

        public Parser(IList<Token> tokens)
        {
            this._tokens = tokens;
            this._index = 0;
            _parsed = new LinkedList<ThreadSpec>();
        }

        public Application Parse()
        {
            
        }

        private Token NextToken()
        {
            try
            {
                return _tokens[++_index];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new NotEnoughTokenException("Token不够", e);
            }
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
    }
}