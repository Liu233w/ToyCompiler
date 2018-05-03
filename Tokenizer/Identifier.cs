namespace Tokenizer
{
    public class Identifier
    {
        public string Recognized { get; set; }

        public string IdentifierType { get; set; }

        public int LastPosition { get; set; }

        public Identifier(string recognized, string identifierType, int lastPosition)
        {
            Recognized = recognized;
            IdentifierType = identifierType;
            LastPosition = lastPosition;
        }
    }
}