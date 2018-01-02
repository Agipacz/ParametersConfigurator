namespace ParametersConfiguratorApplication.Model
{
    public enum LogicalConnector : byte { AND, OR }

    public abstract class LogicalObject
    {
        public abstract bool ComputeLogicalValue();
        public abstract LogicalObject ComputeErrorsStacks(LogicalConnector connector); 

        public bool LogicalValue
        {
            get
            {
                return ComputeLogicalValue();
            }
        }
    }
}
