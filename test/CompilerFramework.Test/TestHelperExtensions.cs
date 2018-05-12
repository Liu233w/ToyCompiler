using System;
using LanguageExt;
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

        // ReSharper disable InconsistentNaming
        public static void ShouldBeLeft<L, R>(this Either<L, R> self, Action<L> action)
        // ReSharper restore InconsistentNaming
        {
            self.Match(
                Right: _ => throw new ShouldAssertException("Either: Expect Left"),
                Left: action
            );
        }

        // ReSharper disable InconsistentNaming
        public static void ShouldBeRight<L, R>(this Either<L, R> self, Action<R> action)
        // ReSharper restore InconsistentNaming
        {
            self.Match(
                Right: action,
                Left: _ => throw new ShouldAssertException("Either: Expect Right")
            );
        }

        // ReSharper disable InconsistentNaming
        public static void ShouldMatchRight<L, R>(this Either<L, R> self, R right)
        // ReSharper restore InconsistentNaming
        {
            self.ShouldBeRight(res => res.ShouldMatchObject(right));
        }

        // ReSharper disable InconsistentNaming
        public static void ShouldMatchLeft<L, R>(this Either<L, R> self, L left)
        // ReSharper restore InconsistentNaming
        {
            self.ShouldBeLeft(res => res.ShouldMatchObject(left));
        }
    }
}
