using System.Collections.Generic;

namespace Liu233w.Compiler.CompilerFramework.Utils
{
    /// <summary>
    /// 某些扩展函数
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 尝试从字典中获取某个值。假如值不存在，会将其默认值插入到字典中，并返回这个默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetOrSetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue = default(TValue))
        {
            var exist = dict.TryGetValue(key, out var value);
            if (exist)
            {
                return value;
            }
            else
            {
                return dict[key] = defaultValue;
            }
        }
    }
}