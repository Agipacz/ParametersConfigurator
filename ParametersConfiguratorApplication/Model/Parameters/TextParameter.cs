using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParametersConfiguratorApplication.Model
{
    public class TextParameter : Parameter
    {
        private string _regularExpression;

        private ObservableCollection<ParameterValueClass<string>> _parameterValues = new ObservableCollection<ParameterValueClass<string>>();

        public TextParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, string regularExpression) 
                                : base (name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.Text)
        {
            this._regularExpression = regularExpression;
            for (int i = 0; i < minNumberOfValues; i++)
                AddParameterValue();
        }

        public TextParameter(TextParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.Text)
        {
            this._regularExpression = param._regularExpression;
        }

        public string RegularExpression
        {
            get
            {
                return _regularExpression;
            }
        }

        public new string Description
        {
            get
            {
                return "Rodzaj parametru: Tekst\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + "\n" + ComputeBounds()
                       + ((_description != null) ? "\nOpis:\n" + _description : "");
            }
        }

        public ObservableCollection<ParameterValueClass<string>> ParameterValues
        {
            get
            {
                return _parameterValues;
            }
        }
        public override void AddParameterValue()
        {
            _parameterValues.Add(new ParameterValueClass<string>());
            _computedHegiht = ComputeHeight();
        }
        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<string> value = item as ParameterValueClass<string>;
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
            if(_regularExpression != null)
                return "RegEx: " + _regularExpression;
            return "";
        }
        public override double ComputeHeight()
        {
            if (_parameterValues.Count <= 1)
                return 100;
            return (_parameterValues.Count * 22.0) + 56.0;
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
                            foreach (ParameterValueClass<string> value in _parameterValues)
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
                switch(parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Enumeration:
                        {
                            foreach(ParameterValueClass<SelectableItem> item in (parameter as EnumerationParameter).ParameterValues)
                            {
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue == item.ParameterValue.ItemValue)
                                    return true;
                            }
                            return false;
                        }
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as FloatParameter).ParameterValues[0].ParameterValue.ToString())
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as IntegerParameter).ParameterValues[0].ParameterValue.ToString())
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as TextParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("TextParameter - EqualTo - SecondArgument Parameter");
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
                    case TypeOfParameter.Enumeration:
                        {
                            foreach (ParameterValueClass<SelectableItem> item in (parameter as EnumerationParameter).ParameterValues)
                            {
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue == item.ParameterValue.ItemValue)
                                    return false;
                            }
                            return true;
                        }
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as FloatParameter).ParameterValues[0].ParameterValue.ToString())
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as IntegerParameter).ParameterValues[0].ParameterValue.ToString())
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as TextParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("TextParameter - NotEqualTo - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool GreaterThan(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - GreaterThan - SecondArgument Parameter");
        }
        public override bool GreaterOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - GreaterOrEqual - SecondArgument Parameter");
        }
        public override bool LessThan(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - LessThan - SecondArgument Parameter");
        }
        public override bool LessOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - LessOrEqual - SecondArgument Parameter");
        }
        public override bool IsSubset(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - IsSubset - SecondArgument Parameter");
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - EmptyIntersection - SecondArgument Parameter");
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("TextParameter - NotEmptyIntersection - SecondArgument Parameter");
        }
        public override bool EqualTo(string value)
        {
            if(ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue == value)
                    return true;
                return false;
            }
            return true; // czy aby na pewno ?
            
        }
        public override bool NotEqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue != value)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterThan(string value)
        {
            throw new InapplicableConditionException("TextParameter - GreaterThan - SecondArgument Value");
        }
        public override bool GreaterOrEqual(string value)
        {
            throw new InapplicableConditionException("TextParameter - GreaterOrEqual - SecondArgument Value");
        }
        public override bool LessThan(string value)
        {
            throw new InapplicableConditionException("TextParameter - LessThan - SecondArgument Value");
        }
        public override bool LessOrEqual(string value)
        {
            throw new InapplicableConditionException("TextParameter - LessOrEqual - SecondArgument Value");
        }
        public override bool RegEx(string value)
        {
            foreach(ParameterValueClass<string> text in _parameterValues)
            {
                if (text.ParameterValue != default(string))
                {
                    if (!Regex.IsMatch(text.ParameterValue, value))
                        return false;
                }
                else
                    return false;
            }
            return true;
        }
        public override bool IsSubset(List<string> set)
        {
            foreach(ParameterValueClass<string> value in ParameterValues)
            {
                if (!set.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool EmptyIntersection(List<string> set)
        {
            foreach (ParameterValueClass<string> value in ParameterValues)
            {
                if (set.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            foreach (ParameterValueClass<string> value in ParameterValues)
            {
                if (set.Contains(value.ParameterValue))
                    return true;
            }
            return false;
        }
        #endregion
    }


}
