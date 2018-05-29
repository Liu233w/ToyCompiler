using System;
using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Utils;

namespace Liu233w.Compiler.CompilerFramework.Parser
{
    /// <summary>
    /// 表示语法规则的定义
    /// </summary>
    public class BnfDefination
    {
        /// <summary>
        /// 规则集内容。Key 表示非终结符， Value 中的每一项都表示此非终结符的推导式。
        /// 推导式中可以包含终结符或非终结符。如果是终结符，会被作为 <see cref="Tokenizer.Token.TokenType"/>
        /// </summary>
        public Dictionary<string, List<string[]>> Rules { get; }

        /// <summary>
        /// 非终结符的 First 集，必须在调用了 <see cref="BuildAnalysisSet"/> 之后才能使用
        /// </summary>
        public Dictionary<string, HashSet<string>> FirstSet { get; private set; }

        /// <summary>
        /// 非终结符的 Follow 集，必须在调用了 <see cref="BuildAnalysisSet"/> 之后才能使用
        /// </summary>
        public Dictionary<string, HashSet<string>> FollowSet { get; private set; }

        /// <summary>
        /// 表示某个非终结符在规则集中出现的位置，便于之后分析过程中进行索引。必须在调用了 <see cref="BuildContainedInField"/> 之后使用
        /// </summary>
        public Dictionary<string, List<(string nonTerminal, int ruleIndex, int position)>> ContainedIn
        { get; private set; }

        public BnfDefination()
        {
            Rules = new Dictionary<string, List<string[]>>();
        }

        /// <summary>
        /// 添加一条语法规则。
        /// 可以给同一个非终结符添加多条推导规则，只需要重复调用此函数即可。
        /// 假如某个字符串出现在 nonTerminal 位置，即表示此字符串之后出现的位置都是非终结符。
        /// 终结符为 TokenType，也就是说，非终结符的命名不能与 TokenType 相同。
        /// </summary>
        /// <param name="nonTerminal">产生式前面的非终结符</param>
        /// <param name="defination">产生式的推导规则。内容可以是终结符或非终结符，可以使用 <see cref="TerminalConsts"/> 中定义的终结符；假如为空数组，则表示Epsilon</param>
        /// <returns></returns>
        public BnfDefination AddRule(string nonTerminal, string[] defination)
        {
            if (Rules.ContainsKey(nonTerminal))
            {
                Rules[nonTerminal].Add(defination);
            }
            else
            {
                Rules.Add(nonTerminal, new List<string[]> { defination });
            }

            return this;
        }

        /// <summary>
        /// 检查某 symbol 是否是终结符
        /// </summary>
        public bool IsTerminal(string symbol)
        {
            return !IsNonTerminal(symbol);
        }

        /// <summary>
        /// 检查某 symbol 是否是非终结符
        /// </summary>
        public bool IsNonTerminal(string symbol)
        {
            return Rules.ContainsKey(symbol);
        }

        /// <summary>
        /// 获取非终结符的所有 BNF 产生式
        /// </summary>
        public List<string[]> GetRulesForNonTerminal(string nonTerminal)
        {
            return Rules[nonTerminal];
        }

        /// <summary>
        /// 构造 <see cref="ContainedIn"/>
        /// </summary>
        public void BuildContainedInField()
        {
            ContainedIn = new Dictionary<string, List<(string nonTerminal, int ruleIndex, int position)>>();
            foreach (var nonTerminalRules in Rules)
            {
                for (var ruleIndex = 0; ruleIndex < nonTerminalRules.Value.Count; ++ruleIndex)
                {
                    var rule = nonTerminalRules.Value[ruleIndex];
                    for (var i = 0; i < rule.Length; ++i)
                    {
                        var symbol = rule[i];
                        if (Rules.ContainsKey(symbol))
                        {
                            // 非终结符
                            ContainedIn.GetOrSetDefault(
                                    symbol,
                                    new List<(string nonTerminal, int ruleIndex, int position)>())
                                .Add((nonTerminalRules.Key, ruleIndex, i));
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// 构造语法规则的 First 集合、Follow 集合。
        ///// 只有非左递归的语法规则可以调用此方法。
        ///// </summary>
        //public void BuildAnalysisSet()
        //{
        //    BuildContainedInField();

        //    // 构造 First 集合
        //    FirstSet = new Dictionary<string, HashSet<string>>();
        //    foreach (var nonTerminal in Rules.Keys)
        //    {
        //        // 相当于记忆化搜索
        //        GetFirstSetForNonTerminal(nonTerminal);
        //    }

        //    // 构造 Follow 集合
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 构造并保存某个非终结符的 First 集合，同时返回此集合。
        ///// 如果集合已经存在，则直接返回。
        ///// </summary>
        ///// <param name="nonTerminal"></param>
        ///// <returns></returns>
        //private HashSet<string> GetFirstSetForNonTerminal(string nonTerminal)
        //{
        //    var setExisted = FirstSet.TryGetValue(nonTerminal, out var set);
        //    if (setExisted) return set;

        //    var nonTerminalRules = Rules[nonTerminal];
        //    set = new HashSet<string>();

        //    foreach (var rule in nonTerminalRules)
        //    {
        //        // 假如第一个符号是一个含有 ɛ 的非终结符，就应该继续遍历后面的符号
        //        foreach (var symbol in rule)
        //        {
        //            if (Rules.ContainsKey(symbol))
        //            {
        //                // 非终结符
        //                var subSet = GetFirstSetForNonTerminal(symbol);
        //                // 假如本符号是含有 ɛ 的非终结符，就应该继续遍历后面的符号
        //                if (subSet.Contains(TerminalConsts.Epsilon))
        //                {
        //                    var alreadyHaveEmptyString = set.Contains(TerminalConsts.Epsilon);
        //                    set.UnionWith(subSet);
        //                    if (!alreadyHaveEmptyString)
        //                    {
        //                        set.Remove(TerminalConsts.Epsilon);
        //                    }
        //                }
        //                else
        //                {
        //                    // 个人感觉这里使用 goto 能提高可读性
        //                    goto check_next_rule;
        //                }
        //            }
        //            else
        //            {
        //                // 终结符
        //                set.Add(rule[0]);
        //                goto check_next_rule;
        //            }
        //        }

        //        // 遍历完了每个 symbol，都是非终结符且含有 ɛ
        //        set.Add(TerminalConsts.Epsilon);

        //        check_next_rule:;
        //    }

        //    FirstSet[nonTerminal] = set;
        //    return set;
        //}
    }
}