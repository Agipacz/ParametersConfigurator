using System;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class ConditionalSection : LogicalObject
    {
        private List<LogicalObject> _subsections;
        private LogicalConnector _logicalConnector;

        public ConditionalSection(List<LogicalObject> subsections, LogicalConnector logicalConnector)
        {
            this._subsections = subsections;
            this._logicalConnector = logicalConnector;
        }

        public List<LogicalObject> Subsections
        {
            get
            {
                return _subsections;
            }
        }

        public LogicalConnector LogicalConnector
        {
            get
            {
                return _logicalConnector;
            }
        }

        public override bool ComputeLogicalValue()
        {
            switch (_logicalConnector)
            {
                case LogicalConnector.AND:
                    {
                        foreach (LogicalObject logicalObject in _subsections)
                        {
                            if (!logicalObject.LogicalValue)
                                return false;
                        }
                        return true;
                    }
                case LogicalConnector.OR:
                    {
                        foreach (LogicalObject logicalObject in _subsections)
                        {
                            if (logicalObject.LogicalValue)
                                return true;
                        }
                        return false;
                    }
                default:
                    {
                        throw new InapplicableConditionException("Nieobsługiwany rodzaj działania logicznego. " + _logicalConnector.ToString());
                    }
            }
        }

        public override LogicalObject ComputeErrorsStacks(LogicalConnector connector)
        {
            ConditionalSection errorSections = new ConditionalSection(new List<LogicalObject>(), _logicalConnector);
            switch (_logicalConnector)
            {
                case LogicalConnector.AND:
                    {
                        foreach (LogicalObject logObject in _subsections)
                        {
                            if (!logObject.LogicalValue)
                            {
                                errorSections.Subsections.Add(logObject.ComputeErrorsStacks(LogicalConnector.AND));
                            }
                        }
                        return errorSections;
                    }
                case LogicalConnector.OR:
                    {
                        foreach (LogicalObject logObject in _subsections)
                        {
                            if (logObject.LogicalValue)
                            {
                                errorSections.Subsections.Add(new TrueLogicalObject());
                                return errorSections;
                            }
                        }
                        List<LogicalObject> subsections = new List<LogicalObject>();
                        foreach (LogicalObject logObject in _subsections)
                        {
                            subsections.Add(logObject.ComputeErrorsStacks(LogicalConnector.OR));
                        }
                        foreach(LogicalObject section in subsections)
                        {
                            if(section as FalseLogicalObject == null)
                            {
                                errorSections.Subsections.AddRange(subsections);
                                return errorSections;
                            }
                        }
                        if (connector == LogicalConnector.AND)
                            return new TrueLogicalObject();
                        else if(connector == LogicalConnector.OR)
                            return new FalseLogicalObject();
                        else
                            throw new Exception("Nieobsługiwany rodzaj działania logicznego. " + connector.ToString());
                    }
                default:
                    {
                        throw new Exception("Nieobsługiwany rodzaj działania logicznego. " + _logicalConnector.ToString());
                    }
            }
        }

        public override string ToString()
        {
            return "Sekcja " + _logicalConnector.ToString();
        }
    }
}
