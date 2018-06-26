using System.Collections.Generic;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    /// <summary>
    /// 这个对象在代码生成中一直存在。假如某个过程需要储存全局数据，可以使用本类来创建一个“层”来储存数据。
    /// 使用这个类可以避免嵌套结构中数据被覆盖的情况。
    /// </summary>
    public class IdTable
    {
        private readonly LinkedList<HashSet<string>> _layers = new LinkedList<HashSet<string>>();

        /// <summary>
        /// 从最外层开始查找Id。如果找不到，返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool LookUpId(string id)
        {
            foreach (var layer in _layers)
            {
                if (layer.Contains(id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 添加一层数据
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(HashSet<string> layer)
        {
            _layers.AddFirst(layer);
        }

        /// <summary>
        /// 移除最外层数据
        /// </summary>
        public void PopLayer()
        {
            _layers.RemoveFirst();
        }
    }
}