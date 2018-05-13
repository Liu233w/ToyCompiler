@echo off

CHCP 65001

echo 将会把运行结果写入到本目录下的 tokenOut.txt 文件中

echo 运行本程序需要 .net core sdk 2.0 以上

echo 编辑此脚本以修改需要分析的源代码文件

pause

dotnet run -p ..\..\src\Experiments\EX1 -- .\词法分析程序实验要求\测试输入源文件\test1.txt >tokenOut.txt 2>&1

echo 运行结束
pause
