using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Tokenizer.Test
{
    public class Tokenzier_Test
    {
        private readonly Tokenizer _tokenizer;

        public Tokenzier_Test()
        {
            _tokenizer = new Tokenizer();
        }

        private void CheckTokensWithoutSpace(IEnumerable<Identifier> identifiers,
            (string type, string recognized)[] needs)
        {
            var idx = 0;
            foreach (var identifier in identifiers)
            {
                if (identifier.IdentifierType == "space")
                {
                    continue;
                }
                identifier.IdentifierType.ShouldBe(needs[idx].type);
                identifier.Recognized.ShouldBe(needs[idx].recognized);

                if (++idx > needs.Length)
                {
                    throw new Exception($"长度不对，应有 {needs.Length} 个 Token");
                }
            }

            idx.ShouldBe(needs.Length);
        }

        [Fact]
        public void Tokenizer_能正确分词()
        {
            var identifiers = _tokenizer.Tokenize(@"
            aName    /*adfdsfsd */
").ToArray();

            identifiers.Length.ShouldBe(5);

            identifiers[0].IdentifierType.ShouldBe("space");
            identifiers[0].Recognized.ShouldBe("\r\n            ");

            identifiers[1].IdentifierType.ShouldBe("name");
            identifiers[1].Recognized.ShouldBe("aName");

            identifiers[2].IdentifierType.ShouldBe("space");
            identifiers[2].Recognized.ShouldBe("    ");

            identifiers[3].IdentifierType.ShouldBe("comment");
            identifiers[3].Recognized.ShouldBe("/*adfdsfsd */");

            identifiers[4].IdentifierType.ShouldBe("space");
            identifiers[4].Recognized.ShouldBe("\r\n");
        }

        [Fact]
        public void Tokenizer_能对程序进行分词()
        {
            var identifiers = _tokenizer.Tokenize(@"
SET 123 to a;
set 456 to b;
set a to b;
");

            CheckTokensWithoutSpace(identifiers, new []
            {
                ("name", "SET"), ("integer", "123"), ("name", "to"), ("name", "a"), (";", ";"),
                ("name", "set"), ("integer", "456"), ("name", "to"), ("name", "b"), (";", ";"),
                ("name", "set"), ("name", "a"), ("name", "to"), ("name", "b"), (";", ";"),
            });
        }
    }
}