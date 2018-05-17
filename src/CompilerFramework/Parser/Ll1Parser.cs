using System;
using System.Collections.Generic;

namespace Liu233w.Compiler.CompilerFramework.Parser
{
    using LL1Table = Dictionary<string, Dictionary<string, IEnumerable<string>>>;

    /// <summary>
    /// 使用 LL(1) 算法进行语法分析。
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LL1Parser
    {
        public static LL1Table BuildAnalysisTable(BnfDefination defination)
        {
            throw new NotImplementedException();
        }
    }
}