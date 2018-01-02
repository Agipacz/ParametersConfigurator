using System;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public enum TypeOfClause : byte { EqualTo, NotEqualTo, GreaterThan, GreaterOrEqual, LessThan, LessOrEqual, IsSubset, EmptyIntersection, NotEmptyIntersection, RegEx };
    public enum TypeOfSecondParameter : byte { ParameterName, Value, Set };

    public class Condition : LogicalObject
    {
        private Parameter _firstParameter;
        private Parameter _secondParameter;

        private string _firstParameterName;
        private TypeOfClause _clause;

        private string _secondParameterName;
        private string _secondParameterValue;
        private string _secondParameterSetString;
        private List<string> _secondParameterSet;
        private TypeOfSecondParameter _typeOfSecondParameter;


        public Condition(string firstParameterName, TypeOfClause clause, string secondParameterName, string secondParameterValue, List<string> secondParameterSet, TypeOfSecondParameter typeOfSecondParameter)
        {
            this._firstParameter = null;
            this._firstParameterName = firstParameterName;
            this._clause = clause;
            this._secondParameterName = secondParameterName;
            this._secondParameterValue = secondParameterValue;
            this._secondParameterSetString = null;
            this._secondParameterSet = secondParameterSet;
            this._typeOfSecondParameter = typeOfSecondParameter;
        }

        public string FirstParameterName
        {
            get
            {
                return _firstParameterName;
            }
        }

        public Parameter FirstParameter
        {
            get
            {
                return _firstParameter;
            }
            set
            {
                _firstParameter = value;
            }
        }

        public TypeOfClause Clause
        {
            get
            {
                return _clause;
            }
        }

        public string SecondParameterName
        {
            get
            {
                return _secondParameterName;
            }
        }

        public string SecondParameterValue
        {
            get
            {
                return _secondParameterValue;
            }
        }

        public string SecondParameterSetString
        {
            get
            {
                if (_secondParameterSetString == null)
                {
                    _secondParameterSetString = "[";
                    foreach (string s in _secondParameterSet)
                    {
                        _secondParameterSetString += s + ", ";
                    }
                    _secondParameterSetString += "]";
                }
                return _secondParameterSetString;
            }
        }

        public Parameter SecondParameter
        {
            get
            {
                return _secondParameter;
            }
            set
            {
                _secondParameter = value;
            }
        }

        public TypeOfSecondParameter TypeOfSecondParameter
        {
            get
            {
                return _typeOfSecondParameter;
            }
        }


        public override bool ComputeLogicalValue()
        {
            bool returnedValue = false;
            
            switch (_typeOfSecondParameter)
            {
                case TypeOfSecondParameter.Value:
                    {
                        #region Value
                        switch (_clause)
                        {
                            case TypeOfClause.EqualTo:
                                returnedValue = FirstParameter.EqualTo(_secondParameterValue);
                                break;
                            case TypeOfClause.NotEqualTo:
                                returnedValue = FirstParameter.NotEqualTo(_secondParameterValue);
                                break;
                            case TypeOfClause.GreaterThan:
                                returnedValue = FirstParameter.GreaterThan(_secondParameterValue);
                                break;
                            case TypeOfClause.GreaterOrEqual:
                                returnedValue = FirstParameter.GreaterOrEqual(_secondParameterValue);
                                break;
                            case TypeOfClause.LessThan:
                                returnedValue = FirstParameter.LessThan(_secondParameterValue);
                                break;
                            case TypeOfClause.LessOrEqual:
                                returnedValue = FirstParameter.LessOrEqual(_secondParameterValue);
                                break;
                            case TypeOfClause.IsSubset:
                                throw new InapplicableConditionException("Warunek nie ma zastosowania (IsSubset dla Value");
                            case TypeOfClause.EmptyIntersection:
                                throw new InapplicableConditionException("Warunek nie ma zastosowania (EmptyIntersection dla Value");
                            case TypeOfClause.NotEmptyIntersection:
                                throw new InapplicableConditionException("Warunek nie ma zastosowania (NotEmptyIntersection dla Value");
                            case TypeOfClause.RegEx:
                                returnedValue = FirstParameter.RegEx(_secondParameterValue);
                                break;
                            default:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli. " + _clause);
                        }
                        break;
                        #endregion
                    }
                case TypeOfSecondParameter.ParameterName:
                    {
                        #region ParameterName
                        switch (_clause)
                        {
                            case TypeOfClause.EqualTo:
                                returnedValue = FirstParameter.EqualTo(SecondParameter);
                                break;
                            case TypeOfClause.NotEqualTo:
                                returnedValue = FirstParameter.NotEqualTo(SecondParameter);
                                break;
                            case TypeOfClause.GreaterThan:
                                returnedValue = FirstParameter.GreaterThan(SecondParameter);
                                break;
                            case TypeOfClause.GreaterOrEqual:
                                returnedValue = FirstParameter.GreaterOrEqual(SecondParameter);
                                break;
                            case TypeOfClause.LessThan:
                                returnedValue = FirstParameter.LessThan(SecondParameter);
                                break;
                            case TypeOfClause.LessOrEqual:
                                returnedValue = FirstParameter.LessOrEqual(SecondParameter);
                                break;
                            case TypeOfClause.IsSubset:
                                returnedValue = FirstParameter.IsSubset(SecondParameter);
                                break;
                            case TypeOfClause.EmptyIntersection:
                                returnedValue = FirstParameter.EmptyIntersection(SecondParameter);
                                break;
                            case TypeOfClause.NotEmptyIntersection:
                                returnedValue = FirstParameter.NotEmptyIntersection(SecondParameter);
                                break;
                            case TypeOfClause.RegEx:
                                #region RegEx
                                throw new InapplicableConditionException("Warunek nie ma zastosowania (RegEx dla ParameterName");
                            #endregion
                            default:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli. " + _clause);
                        }
                        break;
                        #endregion
                    }
                case TypeOfSecondParameter.Set:
                    {
                        #region Set
                        switch (_clause)
                        {
                            case TypeOfClause.EqualTo:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.NotEqualTo:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.GreaterThan:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.GreaterOrEqual:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.LessThan:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.LessOrEqual:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli dla warunku zbiorowego - " + _clause + ". Dopuszczalne klauzule: IsSubset, EmptyIntersection, NotEmptyIntersection");
                            case TypeOfClause.IsSubset:
                                returnedValue = FirstParameter.IsSubset(_secondParameterSet);
                                break;
                            case TypeOfClause.EmptyIntersection:
                                returnedValue = FirstParameter.EmptyIntersection(_secondParameterSet);
                                break;
                            case TypeOfClause.NotEmptyIntersection:
                                returnedValue = FirstParameter.NotEmptyIntersection(_secondParameterSet);
                                break;
                            case TypeOfClause.RegEx:
                                #region RegEx
                                throw new InapplicableConditionException("Warunek nie ma zastosowania (RegEx dla Set");
                            #endregion
                            default:
                                throw new InapplicableConditionException("Nieobsługiwany typ klauzuli. " + _clause);
                        }
                        break;
                        #endregion
                    }
                default:
                    throw new InapplicableConditionException("Nieobsługiwany rodzaj drugiego parametru w warunku. " + _typeOfSecondParameter);
            }
            return returnedValue;
        }

        public string AddErrorToStack()
        {
            string returnedString = FirstParameter.Label + " " + Clause.ToString() + " ";
            switch (TypeOfSecondParameter)
            {
                case TypeOfSecondParameter.Value:
                    returnedString += SecondParameterValue;
                    break;
                case TypeOfSecondParameter.ParameterName:
                    returnedString += SecondParameter.Label;
                    break;
                case TypeOfSecondParameter.Set:
                    returnedString += SecondParameterSetString;
                    break;
            }
            return returnedString;
        }
        public override LogicalObject ComputeErrorsStacks(LogicalConnector connector)
        {
            if (FirstParameter.IsVisible && FirstParameter.IsUsed == ParameterUsageType.Use)
            {
                if (_typeOfSecondParameter == TypeOfSecondParameter.ParameterName)
                {
                    if (SecondParameter.IsVisible && SecondParameter.IsUsed == ParameterUsageType.Use)
                        return this;
                }
                else
                    return this;
            }
            if (connector == LogicalConnector.AND)
                return new TrueLogicalObject();
            else if (connector == LogicalConnector.OR)
                return new FalseLogicalObject();
            else
                throw new Exception("Opcjonalny parametr wywołany z nieistniejącym łącznikiem logicznym.");

        }

        public override string ToString()
        {
            switch (TypeOfSecondParameter)
            {
                case TypeOfSecondParameter.Value:
                    return "<" + FirstParameter.Label + "> " + Clause + " " + SecondParameterValue;
                case TypeOfSecondParameter.Set:
                    return "<" + FirstParameter.Label + "> " + Clause + " " + SecondParameterSetString;
                case TypeOfSecondParameter.ParameterName:
                    return "<" + FirstParameter.Label + "> " + Clause + " <" + SecondParameter.Label + ">";
                default:
                    return "<" + FirstParameter.Label + "> " + Clause + " " + TypeOfSecondParameter.ToString();
            }
        }
    }
}
