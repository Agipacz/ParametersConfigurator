namespace ParametersConfiguratorApplication.Model
{
    public class TrueLogicalObject : LogicalObject
    {
        public override bool ComputeLogicalValue()
        {
            return true;
        }

        public override LogicalObject ComputeErrorsStacks(LogicalConnector connector)
        {
            return this;
        }

        public override string ToString()
        {
            return "TRUE";
        }
    }
}
