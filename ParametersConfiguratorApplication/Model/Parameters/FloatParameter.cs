using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class FloatParameter : Parameter
    {
        private double _minValue;
        private double _maxValue;

        private ObservableCollection<ParameterValueClass<double>> _parameterValues = new ObservableCollection<ParameterValueClass<double>>();

        public FloatParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, double minValue, double maxValue) 
                                : base (name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.Float)
        {
            this._minValue = minValue;
            this._maxValue = maxValue;
            for (int i = 0; i < minNumberOfValues; i++)
                AddParameterValue();
        }

        public FloatParameter(FloatParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.Float)
        {
            this._minValue = param._minValue;
            this._maxValue = param._maxValue;
        }

        public double MinValue
        {
            get
            {
                return _minValue;
            }
        }

        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
        }

        public new string Description
        {
            get
            {
                return "Rodzaj parametru: Liczba rzeczywista\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + "\nOgraniczenia: " + ComputeBounds()
                       + ((_description != null) ? "\nOpis:\n" + _description : "");
            }
        }

        public ObservableCollection<ParameterValueClass<double>> ParameterValues
        {
            get
            {
                return _parameterValues;
            }
        }

        public override void AddParameterValue()
        {
            _parameterValues.Add(new ParameterValueClass<double>());
            _computedHegiht = ComputeHeight();
        }

        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<double> value = item as ParameterValueClass<double>;
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
            string bounds = "Brak ograniczeń zakresu";
            if (_minValue != Double.MinValue)
                bounds = "Min: " + _minValue.ToString();
            if (_maxValue != Double.MaxValue)
                bounds += " Max: " + _maxValue.ToString();
            return bounds;
        }

        public override double ComputeHeight()
        {
            if (_parameterValues.Count <= 1)
                return 95;
            return (_parameterValues.Count * 24.0) + 50.0;
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
                            foreach (ParameterValueClass<double> value in _parameterValues)
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
                    case TypeOfParameter.Enumeration:
                        {
                            foreach (ParameterValueClass<SelectableItem> item in (parameter as EnumerationParameter).ParameterValues)
                            {
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue == Double.Parse(item.ParameterValue.ItemValue.Replace(".", ",")))
                                    return true;
                            }
                            return false;
                        }
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue == Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - EqualTo - SecondArgument Parameter");
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
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue == Double.Parse(item.ParameterValue.ItemValue.Replace(".", ",")))
                                    return false;
                            }
                            return true;
                        }
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue != (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue != Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - NotEqualTo - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool GreaterThan(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue > (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue > (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue > Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - GreaterThan - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool GreaterOrEqual(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue >= (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue >= (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue >= Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - GreaterOrEqual - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool LessThan(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue < (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue < (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue < Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - LessThan - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool LessOrEqual(Parameter parameter)
        {
            if (ParameterValues.Count != 0)
            {
                switch (parameter.TypeOfParameter)
                {
                    case TypeOfParameter.Float:
                        {
                            if ((parameter as FloatParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue <= (parameter as FloatParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Integer:
                        {
                            if ((parameter as IntegerParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue <= (parameter as IntegerParameter).ParameterValues[0].ParameterValue)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue <= Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("FloatParameter - LessOrEqual - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool IsSubset(Parameter parameter)
        {
            throw new InapplicableConditionException("FloatParameter - IsSubset - SecondArgument Parameter");
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("FloatParameter - EmptyIntersection - SecondArgument Parameter");
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("FloatParameter - NotEmptyIntersection - SecondArgument Parameter");
        }
        public override bool EqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue == Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno ?

        }
        public override bool NotEqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue != Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterThan(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue > Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterOrEqual(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue >= Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool LessThan(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue < Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool LessOrEqual(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue <= Double.Parse(value.Replace(".", ",")))
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool RegEx(string value)
        {
            throw new InapplicableConditionException("FloatParameter - RegEx - SecondArgument Value");
        }
        public override bool IsSubset(List<string> set)
        {
            string messageError = "";
            List<double> doubleSet = new List<double>();
            foreach (string s in set)
            {
                try
                {
                    doubleSet.Add(Double.Parse(s.Replace(".", ",")));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako double wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<double> value in ParameterValues)
            {

                if (!doubleSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool EmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<double> doubleSet = new List<double>();
            foreach (string s in set)
            {
                try
                {
                    doubleSet.Add(Double.Parse(s.Replace(".", ",")));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako double wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<double> value in ParameterValues)
            {

                if (doubleSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<double> doubleSet = new List<double>();
            foreach (string s in set)
            {
                try
                {
                    doubleSet.Add(Double.Parse(s.Replace(".", ",")));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako double wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<double> value in ParameterValues)
            {

                if (doubleSet.Contains(value.ParameterValue))
                    return true;
            }
            return false;
        }
        #endregion

    }
}
