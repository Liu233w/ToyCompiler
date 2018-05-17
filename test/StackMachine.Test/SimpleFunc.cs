using System.Collections.Generic;
using Liu233w.StackMachine.Instructions;

namespace Liu233w.StackMachine.Test
{
    public class SimpleFunc : StackFrameFunction
    {
        private int _param;

        private List<int> _simpleList;

        private int _variable;

        public SimpleFunc(int param, List<int> simpleList)
            :base(0)
        {
            _param = param;
            _simpleList = simpleList;
        }

        public SimpleFunc(SimpleFunc that)
            : base(that)
        {
            _variable = that._variable;
            _param = that._param;
            // 不对 list 进行深拷贝
            _simpleList = that._simpleList;
        }

        public override object Clone()
        {
            return new SimpleFunc(this);
        }

        protected override FuncInstructionBase StepMove(int pc, object lastReturned)
        {
            switch (pc)
            {
                case 0:
                {
                    _simpleList.Add(_param);
                    _variable = _param;
                    return Continue();
                }
                case 1:
                {
                    ++_variable;
                    if (_variable < 10)
                    {
                        _simpleList.Add(_variable);
                        return GoTo(1);
                    }
                    else
                    {
                        return Return((_param, _variable));
                    }
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}