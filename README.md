# Compiler
编译原理课上敲的代码（随手写的项目、实验课作业）

这个还没有完成，毕竟编译原理课还没上完。

## 运行环境
- .Net Core SDK 2.0 以上
- Windows 或 Linux 均可

## 目录结构

- ./src/CompilerFramework : 包含一些组件，能够通过预先定义的数据结构来生成出编译器。计划包含词法分析器和语法分析器的生成器。
- ./test/CompilerFramework.Test : 上面的项目的测试

- ./src/Experiments/ : 所有的实验课代码
- ./test/Experiments/ : 所有的实验课单元测试

- ./实验课作业 : 实验课题目要求及作业运行脚本（参见此路径下的 README）

其他放在根目录的文件都是之前写的，以后会被重写并移动到 src 和 test 文件夹中。根目录目前包含：
- LexicalAnalyzer[.Test]: 用 DFS+Continuation回溯 编写的语法分析器，时间复杂度 O(n^3)。由于 C# 不支持 Continuation，
  所以我在这里面手工模拟了函数的调用栈来实现 CallCC。再扩充一下就可以分析上面生成的 Token 了。这个以后会被重写并放进 src 文件夹中。
