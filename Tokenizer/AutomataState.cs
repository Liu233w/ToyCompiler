using System;
using System.Collections.Generic;

namespace Tokenizer
{
    public class AutomataState
    {
        /// <summary>
        /// 表示到达此状态需要的验证。对于初始状态，此处为 null
        /// </summary>
        public Func<char, bool> Asserter { get; set; } = null;
        
        public IList<AutomataState> Next { get; set; } = new List<AutomataState>();

        /// <summary>
        /// 如果不为null，表示一个终止状态；字符串的内容表示到达终止状态时识别的类型。
        /// </summary>
        public string IdentifierType { get; set; } = null;
    }
}