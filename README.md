# Compiler
编译原理课上敲的代码（随手写的项目、实验课作业）

这个还没有完成，毕竟编译原理课还没上完。

## 运行环境
- .Net Core SDK 2.0 以上
- Windows 或 Linux 均可

## 目录结构

- [./src/CompilerFramework](./src/CompilerFramework): 包含一些组件，能够通过预先定义的数据结构来生成出编译器。计划包含词法分析器和语法分析器的生成器。
- [./test/CompilerFramework.Test](./test/CompilerFramework.Test): 上面的项目的测试

- [./src/Experiments/](./src/Experiments/): 所有的实验课代码
- [./test/Experiments/](./test/Experiments/): 所有的实验课单元测试

- [./CodePiece](./CodePiece) 随笔，随堂作业（测试和代码放到一起了）

- [./src/StackMachine](./src/StackMachine): 一个栈帧虚拟机，模拟了函数的调用栈，需要用侵入式的方式来使用。可以使用 call/cc
- [./test/StackMachine](./test/StackMachine): 单元测试

- [./实验课作业](./实验课作业) : 实验课题目要求及作业运行脚本（参见此路径下的 README）
