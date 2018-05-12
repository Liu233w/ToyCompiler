using System;
using LanguageExt;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;
using Newtonsoft.Json;
using Shouldly;

namespace Liu233w.Compiler.CompilerFramework.Test
{
    /// <summary>
    /// 存放便于测试使用的 Extensions
    /// </summary>
    public static class TestHelperExtensions
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public static void ShouldMatchObject(this object obj, object that)
        {
            obj.ToJsonString().ShouldBe(that.ToJsonString());
        }

        public static void ShouldBeLeft<L, R>(this Either<L, R> self, Action<L> action)
        {
            self.Match(
                Right: _ => throw new ShouldAssertException("Either: Expect Left"),
                Left: action
            );
        }

        public static void ShouldBeRight<L, R>(this Either<L, R> self, Action<R> action)
        {
            self.Match(
                Right: action,
                Left: _ => throw new ShouldAssertException("Either: Expect Right")
            );
        }

        public static void ShouldMatchRight<L, R>(this Either<L, R> self, R right)
        {
            self.ShouldBeRight(res => res.ShouldMatchObject(right));
        }

        public static void ShouldMatchLeft<L, R>(this Either<L, R> self, L left)
        {
            self.ShouldBeLeft(res => res.ShouldMatchObject(left));
        }
    }
}
