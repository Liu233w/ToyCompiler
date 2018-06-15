namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class PortType : NodeBase
    {
    }

    public class DataPort : PortType
    {
        public Reference Reference { get; set; }
    }

    public class EventDataPort : PortType
    {
        public Reference Reference { get; set; }
    }

    public class EventPort : PortType
    {
    }
}