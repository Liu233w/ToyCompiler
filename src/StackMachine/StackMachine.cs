using System;
using System.Collections.Generic;
using Liu233w.StackMachine.Exceptions;
using Liu233w.StackMachine.Instructions;

namespace Liu233w.StackMachine
{
    /// <summary>
    /// 栈帧虚拟机，用于运行 <see cref="StackFrameFunction"/>
    /// </summary>
    public class StackMachine
    {
        private Stack<StackFrameFunction> _callingStack = new Stack<StackFrameFunction>();

        /// <summary>
        /// 在虚拟机上运行函数，并返回结果。此操作将清空当前的调用栈。
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public object Run(StackFrameFunction func)
        {
            _callingStack.Clear();
            _callingStack.Push(func);

            StartStepping();

            return ((StackFrameResult)_callingStack.Pop()).Result;
        }

        /// <summary>
        /// 在当前的虚拟机上运行 Continuation
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public object RunWithContinuation(Continuation continuation, object input)
        {
            ResumeWithContinuation(continuation, input);
            StartStepping();
            return ((StackFrameResult)_callingStack.Pop()).Result;
        }

        /// <summary>
        /// 恢复当前续延
        /// </summary>
        /// <param name="cont">要恢复的续延</param>
        /// <param name="result">续延的返回值</param>
        private void ResumeWithContinuation(Continuation cont, object result)
        {
            _callingStack = cont.GetCallingStack();
            ResumeWithResult(result);
        }

        /// <summary>
        /// 运行虚拟机，直到调用栈的顶部是结果为止
        /// </summary>
        private void StartStepping()
        {
            while (_callingStack.Count > 1 || !(_callingStack.Peek() is StackFrameResult))
            {
                object result = null;

                if (_callingStack.Peek() is StackFrameResult resultFrame)
                {
                    _callingStack.Pop();
                    result = resultFrame.Result;
                }

                var instruction = _callingStack.Peek().StepMove(result);
                ExecInstruction(instruction);
            }
        }

        private void ExecInstruction(FuncInstructionBase instruction)
        {
            switch (instruction)
            {
                case NoneInstruction _:
                {
                    // Nothing here
                    break;
                }
                case ReturnInstruction ret:
                {
                    HandleFunctionReturn(ret.Result);
                    break;
                }
                case CallFuncInstruction call:
                {
                    PushStack(call.Func);
                    break;
                }
                case CallCcInstruction callcc:
                {
                    HandleCallCc(callcc.Lambda);
                    break;
                }
                case ResumeContinuationInstruction resumeContinuation:
                {
                    ResumeWithContinuation(resumeContinuation.Cont, resumeContinuation.Input);
                    break;
                }
                default:
                {
                    throw new InvalidInstructionException("不支持此指令");
                }
            }
        }

        private void PushStack(StackFrameFunction func)
        {
            _callingStack.Push(func);
        }

        /// <summary>
        /// 处理 CallCc 操作
        /// </summary>
        /// <returns></returns>
        private void HandleCallCc(Func<Continuation, object> lambda)
        {
            var result = lambda(new Continuation(_callingStack));
            ResumeWithResult(result);
        }

        /// <summary>
        /// 用一个结果栈帧来代替栈顶的函数栈帧
        /// </summary>
        /// <param name="result"></param>
        private void HandleFunctionReturn(object result)
        {
            _callingStack.Pop();
            ResumeWithResult(result);
        }

        /// <summary>
        /// 在栈顶放入一个结果栈帧
        /// </summary>
        /// <param name="result"></param>
        private void ResumeWithResult(object result)
        {
            _callingStack.Push(new StackFrameResult(result));
        }
    }
}