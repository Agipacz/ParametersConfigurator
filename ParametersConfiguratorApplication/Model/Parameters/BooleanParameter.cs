using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class BooleanParameter : Parameter
    {
        private ObservableCollection<ParameterValueClass<bool>> _parameterValues = new ObservableCollection<ParameterValueClass<bool>>();

        public BooleanParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues)
                                : base(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.Boolean)
        {
            for (int i = 0; i < minNumberOfValues; i++)
                AddParameterValue();
        }

        public BooleanParameter(BooleanParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.Boolean)
        {

        }

        public ObservableCollection<ParameterValueClass<bool>> ParameterValues
        {
            get
            {
                return _parameterValues;
            }
        }

        public new string Description
        {
            get
            {
                return "Rodzaj parametru: TAK/NIE\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + ((_description != null) ? "\nOpis:\n" + _description : "");
            }
        }

        public override void AddParameterValue()
        {
            _parameterValues.Add(new ParameterValueClass<bool>());
            _computedHegiht = ComputeHeight();
        }

        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<bool> value = item as ParameterValueClass<bool>;
            _parameterValues.Remove(value);
            _computedHegiht = ComputeHeight();
        }

        public override int ParameterValuesCount()
        {
            return _parameterValues.Count;
        }

        public override bool IsParameterValuesButtonsVisible()
        {
            if (MinNumberOfValues == MaxNumberOfValues)
                return false;
            return true;
        }

        public override string ComputeBounds()
        {
            return "";
        }
        
        public override double ComputeHeight()
        {
            if (_parameterValues.Count <= 1)
                return 95;
            return (_parameterValues.Count * 22.0) + 50.0;
        }
        public override string ToFileSaveTxt(ViewModel.SavingType savingType)
        {
            string sign = SavingType(savingType);

            switch (IsUsed)
            {
                case ParameterUsageType.Use:
                    {
                        string returnedString = Name + "=";
                        if (_parameterValues.Count == 1)
                            returnedString += sign + _parameterValues[0].ToString() + sign;
                        else
                        {
                            returnedString += SignsOfSet._beginningSign;
                            bool notFirstValueFlag = false;
                            foreach (ParameterValueClass<bool> value in _parameterValues)
                            {
                                if (notFirstValueFlag)
                                    returnedString += SignsOfSet._separatingSign;
                                else
                                    notFirstValueFlag = true;
                                returnedString += sign + value.ParameterValue.ToString() + sign;
                            }
                            returnedString += SignsOfSet._endingSign;
                        }
                        return returnedString + "\n";
                    }
                case ParameterUsageType.UseEmpty:
                    {
                        return Name + "=\n";
                    }
                case ParameterUsageType.Skip:
                    {
                        return "";
                    }
                default:
                    throw new Exception("Błędna wartość właściwości IsUsed.");
            }
        }
        
        #region ComparasionMethods
        public override bool EqualTo(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Boolean:
                        {
                            if ((parameter as BooleanParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as BooleanParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("BooleanParameter - EqualTo - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool NotEqualTo(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Boolean:
                        {
                            if ((parameter as BooleanParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as BooleanParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("BooleanParameter - NotEqualTo - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool GreaterThan(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - GreaterThan - SecondArgument Parameter");
        }
        public override bool GreaterOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - GreaterOrEqual - SecondArgument Parameter");
        }
        public override bool LessThan(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - LessThan - SecondArgument Parameter");
        }
        public override bool LessOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - LessOrEqual - SecondArgument Parameter");
        }
        public override bool IsSubset(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - IsSubset - SecondArgument Parameter");
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - EmptyIntersection - SecondArgument Parameter");
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("BooleanParameter - NotEmptyIntersection - SecondArgument Parameter");
        }
        public override bool EqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue == bool.Parse(value))
                    return true;
                return false;
            }
            return true; // czy aby na pewno ?

        }
        public override bool NotEqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue != bool.Parse(value))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterThan(string value)
        {
            throw new InapplicableConditionException("BooleanParameter - GreaterThan - SecondArgument Value");
        }
        public override bool GreaterOrEqual(string value)
        {
            throw new InapplicableConditionException("BooleanParameter - GreaterOrEqual - SecondArgument Value");
        }
        public override bool LessThan(string value)
        {
            throw new InapplicableConditionException("BooleanParameter - LessThan - SecondArgument Value");
        }
        public override bool LessOrEqual(string value)
        {
            throw new InapplicableConditionException("BooleanParameter - LessOrEqual - SecondArgument Value");
        }
        public override bool RegEx(string value)
        {
            throw new InapplicableConditionException("BooleanParameter - RegEx - SecondArgument Value");
        }
        public override bool IsSubset(List<string> set)
        {
            throw new InapplicableConditionException("BooleanParameter - IsSubset - SecondArgument Set");
        }
        public override bool EmptyIntersection(List<string> set)
        {
            throw new InapplicableConditionException("BooleanParameter - EmptyIntersection - SecondArgument Set");
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            throw new InapplicableConditionException("BooleanParameter - NotEmptyIntersection - SecondArgument Set");
        }
        #endregion
    }
}
