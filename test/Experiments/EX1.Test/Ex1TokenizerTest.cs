using System;
using System.Collections.Generic;
using System.Linq;
using Liu233w.Compiler.CompilerFramework.Test;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX1.libs;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Liu233w.Compiler.EX1.Test
{
    public class Ex1TokenizerTest
    {
        [Fact]
        public void TokenizeBuffer_能够正确解析字符串()
        {
            var buffer = @"
thread Thread1
features
	AP_Position_Input : in event data port Nav_Types::Position_GPS;
flows
        flow1: flow path signal -> result1;
properties
	dispatch_protocol => access 50.0;
end Thread1;
";

            var expectedSnapshhot =
                new[]
                {
                    ("thread", "thread"),
                    ("Thread1", "identifier"),
                    ("features", "features"),
                    ("AP_Position_Input", "identifier"),
                    (":", ":"),
                    ("in", "in"),
                    ("event", "event"),
                    ("data", "data"),
                    ("port", "port"),
                    ("Nav_Types", "identifier"),
                    ("::", "::"),
                    ("Position_GPS", "identifier"),
                    (";", ";"),
                    ("flows", "flows"),
                    ("flow1", "identifier"),
                    (":", ":"),
                    ("flow", "flow"),
                    ("path", "path"),
                    ("signal", "identifier"),
                    ("->", "->"),
                    ("result1", "identifier"),
                    (";", ";"),
                    ("properties", "properties"),
                    ("dispatch_protocol", "identifier"),
                    ("=>", "=>"),
                    ("access", "access"),
                    ("50.0", "decimal"),
                    (";", ";"),
                    ("end", "end"),
                    ("Thread1", "identifier"),
                    (";", ";"),
                };

            Ex1Tokenizer.TokenizeBuffer(buffer)
                .ShouldBeRight(lst => lst
                    .Select(item => (item.Content, item.TokenType))
                    .ToArray()
                    .ShouldMatchObject(expectedSnapshhot, TypeNameHandling.None)
                );
        }
    }
}