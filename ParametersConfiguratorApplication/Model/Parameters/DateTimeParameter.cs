using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class DateTimeParameter : Parameter
    {
        private DateTime _minValue;
        private DateTime _maxValue;

        private ObservableCollection<ParameterValueClass<DateTime>> _parameterValues = new ObservableCollection<ParameterValueClass<DateTime>>();

        public DateTimeParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, DateTime minValue, DateTime maxValue) 
                                : base (name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.DateTime)
        {
            this._minValue = minValue;
            this._maxValue = maxValue;

            for (int i = 0; i < minNumberOfValues; i++)
                AddParameterValue();
        }
        public DateTimeParameter(DateTimeParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.DateTime)
        {
            this._minValue = param._minValue;
            this._maxValue = param._maxValue;
        }

        public DateTime MinValue
        {
            get
            {
                return _minValue;
            }
        }

        public DateTime MaxValue
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
                return "Rodzaj parametru: Data i godzina\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + "\nOgraniczenia: " + ComputeBounds()
                       + ((_description != null) ? "\nOpis:\n" + _description : "");
            }
        }

        public ObservableCollection<ParameterValueClass<DateTime>> ParameterValues
        {
            get
            {
                return _parameterValues;
            }
        }
        public bool IsTimeRelevant
        {
            get
            {
                return true;
            }
        }

        public override void AddParameterValue()
        {
            if (_minValue == DateTime.MinValue)
                _parameterValues.Add(new ParameterValueClass<DateTime>(DateTime.Today));
            else
                _parameterValues.Add(new ParameterValueClass<DateTime>(_minValue));
            _computedHegiht = ComputeHeight();
        }

        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<DateTime> value = item as ParameterValueClass<DateTime>;
            _parameterValues.Remove(value);
            _computedHegiht = ComputeHeight();
        }

        public override int ParameterValuesCount()
        {
            return _parameterValues.Count;
        }

        public override string ToString()
        {
            return _parameterValues[0].ToString();
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
            if (_minValue != DateTime.MinValue)
                bounds = "Min: " + _minValue.ToString();
            if (_maxValue != DateTime.MaxValue)
                bounds += " Max: " + _maxValue.ToString();
            return bounds;
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
                            foreach (ParameterValueClass<DateTime> value in _parameterValues)
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
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue.Ticks == DateTime.Parse(item.ParameterValue.ItemValue).Ticks)
                                    return true;
                            }
                            return false;
                        }
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks == (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks == (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null &&  ParameterValues[0].ParameterValue.Ticks == DateTime.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue).Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - EqualTo - SecondArgument Parameter");
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
                                if (item.ParameterValue.IsSelected && ParameterValues[0].ParameterValue.Ticks == DateTime.Parse(item.ParameterValue.ItemValue).Ticks)
                                    return false;
                            }
                            return true;
                        }
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks != (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks != (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.Text:
                        {
                            if ((parameter as TextParameter).ParameterValues.Count != 0 && (parameter as TextParameter).ParameterValues[0].ParameterValue != null && ParameterValues[0].ParameterValue.Ticks != DateTime.Parse((parameter as TextParameter).ParameterValues[0].ParameterValue).Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - NotEqualTo - SecondArgument Parameter");
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
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks > (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks > (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - GreaterThan - SecondArgument Parameter");
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
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks >= (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks >= (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - GreaterOrEqual - SecondArgument Parameter");
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
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks < (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks < (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - LessThan - SecondArgument Parameter");
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
                    case TypeOfParameter.Date:
                        {
                            if ((parameter as DateParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks <= (parameter as DateParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    case TypeOfParameter.DateTime:
                        {
                            if ((parameter as DateTimeParameter).ParameterValues.Count != 0 && ParameterValues[0].ParameterValue.Ticks <= (parameter as DateTimeParameter).ParameterValues[0].ParameterValue.Ticks)
                                return true;
                            return false;
                        }
                    default:
                        throw new InapplicableConditionException("DateTimeParameter - LessOrEqual - SecondArgument Parameter");
                }
            }
            return true; // czy aby na pewno ?
        }
        public override bool IsSubset(Parameter parameter)
        {
            throw new InapplicableConditionException("DateTimeParameter - IsSubset - SecondArgument Parameter");
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("DateTimeParameter - EmptyIntersection - SecondArgument Parameter");
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            throw new InapplicableConditionException("DateTimeParameter - NotEmptyIntersection - SecondArgument Parameter");
        }
        public override bool EqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks == DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno ?

        }
        public override bool NotEqualTo(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks != DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterThan(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks > DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool GreaterOrEqual(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks >= DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool LessThan(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks < DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool LessOrEqual(string value)
        {
            if (ParameterValues.Count != 0)
            {
                if (ParameterValues[0].ParameterValue.Ticks <= DateTime.Parse(value).Ticks)
                    return true;
                return false;
            }
            return true; // czy aby na pewno 
        }
        public override bool RegEx(string value)
        {
            throw new InapplicableConditionException("DateTimeParameter - RegEx - SecondArgument Value");
        }
        public override bool IsSubset(List<string> set)
        {
            string messageError = "";
            List<DateTime> dateTimeSet = new List<DateTime>();
            foreach (string s in set)
            {
                try
                {
                    dateTimeSet.Add(DateTime.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako DataTime wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<DateTime> value in ParameterValues)
            {

                if (!dateTimeSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool EmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<DateTime> dateTimeSet = new List<DateTime>();
            foreach (string s in set)
            {
                try
                {
                    dateTimeSet.Add(DateTime.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako DataTime wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<DateTime> value in ParameterValues)
            {

                if (dateTimeSet.Contains(value.ParameterValue))
                    return false;
            }
            return true;
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            string messageError = "";
            List<DateTime> dateTimeSet = new List<DateTime>();
            foreach (string s in set)
            {
                try
                {
                    dateTimeSet.Add(DateTime.Parse(s));
                }
                catch
                {
                    messageError += "Nie mogę sparsować jako DataTime wartości :" + s + "\n";
                }
            }
            foreach (ParameterValueClass<DateTime> value in ParameterValues)
            {

                if (dateTimeSet.Contains(value.ParameterValue))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
