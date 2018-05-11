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
    }
}
