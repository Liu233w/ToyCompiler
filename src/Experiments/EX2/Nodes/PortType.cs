namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class PortType : NodeBase
    {
    }

    public class DataPort : PortType
    {
        public ReferenceBase Reference { get; set; }

        public override string Type { get; } = nameof(DataPort);
    }

    public class EventDataPort : PortType
    {
        public ReferenceBase Reference { get; set; }

        public override string Type { get; } = nameof(EventDataPort);
    }

    public class EventPort : PortType
    {
        public override string Type { get; } = nameof(EventPort);
    }
}