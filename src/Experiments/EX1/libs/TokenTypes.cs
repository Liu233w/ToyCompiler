namespace Liu233w.Compiler.EX1.libs
{
    /// <summary>
    /// 所有的标识符类型
    /// </summary>
    public class TokenTypes
    {
        #region 关键字

        public const string Thread = "thread";

        public const string Features = "features";

        public const string Flows = "flows";

        public const string Properties = "properties";

        public const string End = "end";

        public const string None = "none";

        public const string In = "in";

        public const string Out = "out";

        public const string Data = "data";

        public const string Port = "port";

        public const string Event = "event";

        public const string Parameter = "parameter";

        public const string Flow = "flow";

        public const string Source = "source";

        public const string Sink = "sink";

        public const string Path = "path";

        public const string Constant = "constant";

        public const string Access = "access";

        #endregion

        #region 符号

        public const string Arraw1 = "=>";

        public const string Arraw2 = "+=>";

        public const string Arraw3 = "->";

        public const string Semicolon = ";";

        public const string SingleColon = ":";

        public const string DoubleColon = "::";

        public const string LeftBrace = "{";

        public const string RightBrace = "}";

        #endregion

        public const string Identifier = "identifier";

        public const string Decimal = "decimal";
    }
}