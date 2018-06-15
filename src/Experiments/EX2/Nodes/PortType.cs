namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class PortType : NodeBase
    {
    }

    public class DataPort : PortType
    {
        public ReferenceBase Reference { get; set; }
    }

    public class EventDataPort : PortType
    {
        public ReferenceBase Reference { get; set; }
    }

    public class EventPort : PortType
    {
    }
}