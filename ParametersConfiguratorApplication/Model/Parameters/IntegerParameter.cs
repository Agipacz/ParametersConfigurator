using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class IntegerParameter : Parameter
    {
        private int _minValue;
        private int _maxValue;

        private ObservableCollection<ParameterValueClass<int>> _parameterValues = new ObservableCollection<ParameterValueClass<int>>();

        public IntegerParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, int minValue, int maxValue) 
                                : base (name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.Integer)
        {
            this._minValue = minValue;
            this._maxValue = maxValue;

            for (int i = 0; i < minNumberOfValues; i++)
                AddParameterValue();
        }

        public IntegerParameter(IntegerParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.Integer)
        {
            this._minValue = param._minValue;
            this._maxValue = param._maxValue;
        }


        public new string Description
        {
            get
            {
                return "Rodzaj parametru: Liczba całkowita\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + "\nOgraniczenia: " + ComputeBounds()
                       + ( (_description != null) ? "\nOpis:\n" + _description : "");   
            }
        }

        public int MinValue
        {
            get
            {
                return _minValue;
            }
        }

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }
        }

        public ObservableCollection<ParameterValueClass<int>> ParameterValues
        {
            get
            {
                return _parameterValues;
            }
        }

        public override void AddParameterValue()
        {
            _parameterValues.Add(new ParameterValueClass<int>());
            _computedHegiht = ComputeHeight();
        }

        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<int> value = item as ParameterValueClass<int>;
            _parameterValues.Remove(value);
            _computedHegiht = ComputeHeight();
        }

        //public override string ToString()
        //{
        //    string s = "";
        //    foreach (object v in _parameterValues)
        //        s += v.ToString();
        //    return s;
        //}

        public override string ComputeBounds()
        {
            string bounds = "Brak ograniczeń zakresu";
            if (_minValue != Int32.MinValue)
                bounds = "Min: " + _minValue.ToString();
            if (_maxValue != Int32.MaxValue)
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
                            foreach (ParameterValueClass<int> value in _parameterValues)
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
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue == Double.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue.Replace(".", ",")))
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("IntegerParameter - EqualTo - SecondArgument Parameter");
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
                        throw new InapplicableConditionException("IntegerParameter - NotEqualTo - SecondArgument Parameter");
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
                        throw new InapplicableConditionException("IntegerParameter - GreaterThan - SecondArgument Parameter");
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
                        throw new InapplicableConditionException("IntegerParameter - GreaterOrEqual - SecondArgument Parameter");
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
                        throw new InapplicableConditionException("IntegerParameter - LessThan - SecondArgument Parameter");
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
                        throw new InapplicableConditionException("IntegerParameter - LessOrEqual - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool IsSubset(Parameter parameter)
        {
            throw new InapplicableConditionException("IntegerParameter - IsSubset - SecondArgument Parameter");
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("IntegerParameter - EmptyIntersection - SecondArgument Parameter");
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("IntegerParameter - NotEmptyIntersection - SecondArgument Parameter");
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
            throw new InapplicableConditionException("IntegerParameter - RegEx - SecondArgument Value");
        }
        public override bool IsSubset(List<string> set)
        {
            string messageError = "";
            List<int> intSet = new List<int>();
            foreach (string s in set)
            {
                try
                {
                    intSet.Add(Int32.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako int wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<int> value in ParameterValues)
            {

                if (!intSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool EmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<int> intSet = new List<int>();
            foreach (string s in set)
            {
                try
                {
                    intSet.Add(Int32.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako int wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<int> value in ParameterValues)
            {

                if (intSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<int> intSet = new List<int>();
            foreach (string s in set)
            {
                try
                {
                    intSet.Add(Int32.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako int wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<int> value in ParameterValues)
            {

                if (intSet.Contains(value.ParameterValue))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
