using System;

namespace ParametersConfiguratorApplication.Model
{
    public class InapplicableConditionException : Exception
    {
        public InapplicableConditionException(string err) : base(err) { }
    }
}
