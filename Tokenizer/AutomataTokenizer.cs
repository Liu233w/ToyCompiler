namespace Tokenizer
{
    public static class AutomataTokenizer
    {
        public static Identifier GetByAutomata(AutomataState automataState, string buffer, int start)
        {
            if (start >= buffer.Length)
            {
                return new Identifier("", null, start);
            }

            foreach (var state in automataState.Next)
            {
                if (state.Asserter(buffer[start]))
                {
                    var res = GetByAutomata(state, buffer, start + 1);
                    if (res.Recognized != null)
                    {
                        return new Identifier(buffer[start] + res.Recognized, res.IdentifierType ?? state.IdentifierType, res.LastPosition);
                    }
                }
            }

            if (automataState.IdentifierType != null)
            {
                return new Identifier("", automataState.IdentifierType, start);
            }
            else
            {
                return new Identifier(null, null, start);
            }
        }
    }
}