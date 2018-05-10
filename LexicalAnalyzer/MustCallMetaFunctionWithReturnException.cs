using System;

namespace LexicalAnalyzer
{
    /// <summary>
    /// 在一个 <see cref="StackMachine{TParams,TVariables,TResult}"/> 的步进函数中，
    /// 每次调用 Return, GoTo 之类的函数都必须返回，假如没有返回，会抛出此异常。
    /// </summary>
    public class MustCallMetaFunctionWithReturnException : Exception
    {
        
    }
}