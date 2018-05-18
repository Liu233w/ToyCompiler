一个侵入式的栈帧虚拟机
============================

这个跟 javaflow 类似，只不过不会自动对字节码做变换，而是需要用户自己对自己的代码进行变换，需要将函数的输入和用到的变量使用类成员来保存，
还要手动通过输入的 pc 值来处理控制流。不过好处是不会影响上层的代码，可以通过新建 StackMachine 对象来隔离需要变换的函数和不需要变换的函数。

我实现它主要是为了在 [DfsParser](../CompilerFramework/Parser/DfsParser.cs) 中使用 call/cc。

这个库的使用方式请参考它的[测试用例](../../test/StackMachine.Test/)和 [DfsParser](../CompilerFramework/Parser/DfsParser.cs)