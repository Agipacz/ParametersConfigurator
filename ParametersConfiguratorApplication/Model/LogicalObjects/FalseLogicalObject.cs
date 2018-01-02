namespace ParametersConfiguratorApplication.Model
{
    public class FalseLogicalObject : LogicalObject
    {
        public override bool ComputeLogicalValue()
        {
            return false;
        }
        public override LogicalObject ComputeErrorsStacks(LogicalConnector connector)
        {
            return this;
        }

        public override string ToString()
        {
            return "FALSE";
        }
    }
}
