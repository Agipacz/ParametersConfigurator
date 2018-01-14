using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ParametersConfiguratorApplication.Model
{
    public class EnumerationParameter : Parameter
    {
        private List<string> _items = new List<string>();

        private ObservableCollection<ParameterValueClass<SelectableItem>> _parameterValues = new ObservableCollection<ParameterValueClass<SelectableItem>>();

        public EnumerationParameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, List<string> items)
                                    : base(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, TypeOfParameter.Enumeration)
        {
            this._items = items;
            foreach (string item in items)
                AddParameterValue(new ParameterValueClass<SelectableItem>(new SelectableItem(false, item)));
            _computedHegiht = ComputeHeight();
        }

        public EnumerationParameter(EnumerationParameter param) : base(param.Name, param.Label, param.Description, param.SignsOfSet, param.Sections, param.IsOptional, param.MinNumberOfValues, param.MaxNumberOfValues, TypeOfParameter.Enumeration)
        {
            this._items = param._items;
            _computedHegiht = ComputeHeight();
        }

        public ObservableCollection<ParameterValueClass<SelectableItem>> ParameterValues
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
                return "Rodzaj parametru: Wyliczeniowy\n"
                       + Label + " (" + Name + ")"
                       + "\nLiczebność: " + NumberOf
                       + ((_description != null) ? "\nOpis:\n" + _description : "");
            }
        }

        public bool IsMaxSelected
        {
            get
            {
                if (CountSelectedItem() == MaxNumberOfValues)
                    return true;
                else
                    return false;
            }
        }

        private int CountSelectedItem()
        {
            int count = 0;
            foreach (ParameterValueClass<SelectableItem> item in _parameterValues)
            {
                if (item.ParameterValue.IsSelected)
                    count++;
            }
            return count;
        }

        public override string ComputeBounds()
        {
            return "";
        }
        public override double ComputeHeight()
        {
            if (_parameterValues.Count <= 1)
                return 70;
            return (_parameterValues.Count * 18.0) + 50.0;
        }
        public override void AddParameterValue()
        {
            throw new NotImplementedException();
        }
        public void AddParameterValue(ParameterValueClass<SelectableItem> selectableItem)
        {
            _parameterValues.Add(selectableItem);
        }
        public override void RemoveParameterValue<T>(ParameterValueClass<T> item)
        {
            ParameterValueClass<SelectableItem> value = item as ParameterValueClass<SelectableItem>;
            _parameterValues.Remove(value);
        }
        public override int ParameterValuesCount()
        {
            return _parameterValues.Count;
        }
        public override bool IsParameterValuesButtonsVisible()
        {
            return false;
        }
        public override string ToFileSaveTxt(ViewModel.SavingType savingType)
        {
            string sign = SavingType(savingType);

            switch (IsUsed)
            {
                case ParameterUsageType.Use:
                    {
                        List<string> allSelectedValues = new List<string>();
                        foreach (ParameterValueClass<SelectableItem> value in _parameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                allSelectedValues.Add(value.ParameterValue.ItemValue.ToString());
                            }
                        }
                        string returnedString = Name + "=";
                        if (allSelectedValues.Count == 0)
                            return returnedString + "\n";
                        if (allSelectedValues.Count == 1)
                            returnedString += sign + allSelectedValues[0].ToString() + sign;
                        else
                        {
                            returnedString += SignsOfSet._beginningSign;
                            bool notFirstValueFlag = false;
                            foreach (string value in allSelectedValues)
                            {
                                if (notFirstValueFlag)
                                    returnedString += SignsOfSet._separatingSign;
                                else
                                    notFirstValueFlag = true;
                                returnedString += sign + value + sign;
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
            throw new InapplicableConditionException("EnumerationParameter - EqualTo - SecondArgument Parameter");
        }
        public override bool NotEqualTo(Parameter parameter)
        {
            throw new InapplicableConditionException("EnumerationParameter - NotEqualTo - SecondArgument Parameter");
        }
        public override bool GreaterThan(Parameter parameter)
        {
            throw new InapplicableConditionException("EnumerationParameter - GreaterThan - SecondArgument Parameter");
        }
        public override bool GreaterOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("EnumerationParameter - GreaterOrEqual - SecondArgument Parameter");
        }
        public override bool LessThan(Parameter parameter)
        {
            throw new InapplicableConditionException("EnumerationParameter - LessThan - SecondArgument Parameter");
        }
        public override bool LessOrEqual(Parameter parameter)
        {
            throw new InapplicableConditionException("EnumerationParameter - LessOrEqual - SecondArgument Parameter");
        }
        public override bool IsSubset(Parameter parameter)
        {
            switch (parameter.TypeOfParameter)
            {
                case TypeOfParameter.Date:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = true; // znaleźliśmy że dobrze
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                case TypeOfParameter.DateTime:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateTimeParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = true; // znaleźliśmy że dobrze
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                case TypeOfParameter.Integer:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<int> secondValue in (parameter as IntegerParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = true; // znaleźliśmy że dobrze
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                case TypeOfParameter.Float:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<double> secondValue in (parameter as FloatParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = true; // znaleźliśmy że dobrze
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                case TypeOfParameter.Enumeration:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<SelectableItem> secondValue in (parameter as EnumerationParameter).ParameterValues)
                                {
                                    if (secondValue.ParameterValue.IsSelected)
                                    {
                                        if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ItemValue)
                                        {
                                            returnedValue = true; // znaleźliśmy że dobrze
                                            break;
                                        }
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                case TypeOfParameter.Text:
                    {
                        bool returnedValue = false; // ?? nic nie zaznaczone to błąd
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = false; // coś zaznaczone wiec jeszcze nie wiemy czy dobrze, zakładamy że źle
                                foreach (ParameterValueClass<string> secondValue in (parameter as TextParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = true; // znaleźliśmy że dobrze
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie nie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret false) albo co najmniej jedną znalezioną parą (ret true)
                    }
                default:
                    throw new InapplicableConditionException("EnumerationParameter - IsSubset - SecondArgument Parameter");
            }
        }
        public override bool EmptyIntersection(Parameter parameter)
        {
            switch (parameter.TypeOfParameter)
            {
                case TypeOfParameter.Date:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = false; // znaleźliśmy że źle
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.DateTime:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateTimeParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = false; // znaleźliśmy że źle
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.Integer:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<int> secondValue in (parameter as IntegerParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = false; // znaleźliśmy że źle
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.Float:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<double> secondValue in (parameter as FloatParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                    {
                                        returnedValue = false; // znaleźliśmy że źle
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.Enumeration:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<SelectableItem> secondValue in (parameter as EnumerationParameter).ParameterValues)
                                {
                                    if (secondValue.ParameterValue.IsSelected)
                                    {
                                        if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ItemValue)
                                        {
                                            returnedValue = false; // znaleźliśmy że źle
                                            break;
                                        }
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.Text:
                    {
                        bool returnedValue = true; // ?? nic nie zaznaczone to dobrze
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                returnedValue = true; // coś zaznaczone wiec jeszcze nie wiemy czy źle, zakładamy że dobrze
                                foreach (ParameterValueClass<string> secondValue in (parameter as TextParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue)
                                    {
                                        returnedValue = false; // znaleźliśmy że źle
                                        break;
                                    }
                                }
                                if (!returnedValue)
                                    return returnedValue; // ostanie zaznaczenie znalazło się wewnątrz zbioru z drugiego parametru (ret false)
                            }
                        }
                        return returnedValue; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                default:
                    throw new InapplicableConditionException("EnumerationParameter - EmptyIntersection - SecondArgument Parameter");
            }
        }
        public override bool NotEmptyIntersection(Parameter parameter)
        {
            switch (parameter.TypeOfParameter)
            {
                case TypeOfParameter.Date:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.Date.ToString())
                                    {
                                        return true; // znaleźliśmy że dobrze
                                    }
                                }
                            }
                        }
                        return false; // nie było dopasowań więc fasle
                    }
                case TypeOfParameter.DateTime:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<DateTime> secondValue in (parameter as DateTimeParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                        return true; // znaleźliśmy że dobrze
                                }
                            }
                        }
                        return false; // nie było dopasowań więc fasle
                    }
                case TypeOfParameter.Integer:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<int> secondValue in (parameter as IntegerParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                        return true;
                                }
                            }
                        }
                        return false; // nie było dopasowań więc fasle
                    }
                case TypeOfParameter.Float:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<double> secondValue in (parameter as FloatParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ToString())
                                        return true;
                                }
                            }
                        }
                        return false; // nie było dopasowań więc fasle
                    }
                case TypeOfParameter.Enumeration:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<SelectableItem> secondValue in (parameter as EnumerationParameter).ParameterValues)
                                {
                                    if (secondValue.ParameterValue.IsSelected)
                                    {
                                        if (value.ParameterValue.ItemValue == secondValue.ParameterValue.ItemValue)
                                            return true;
                                    }
                                }
                            }
                        }
                        return false; // zewnętrzny foreach skończył zerem wejśc (ret true) albo nie znaleziono pasujący par (ret true)
                    }
                case TypeOfParameter.Text:
                    {
                        foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
                        {
                            if (value.ParameterValue.IsSelected)
                            {
                                foreach (ParameterValueClass<string> secondValue in (parameter as TextParameter).ParameterValues)
                                {
                                    if (value.ParameterValue.ItemValue == secondValue.ParameterValue)
                                        return true;
                                }
                            }
                        }
                        return false; // nie było dopasowań więc fasle
                    }
                default:
                    throw new InapplicableConditionException("EnumerationParameter - NotEmptyIntersection - SecondArgument Parameter");
            }
        }
        public override bool EqualTo(string value)
        {
            bool returnedValue = false; // jeszcze nic nie zaznaczone (ret false)
            foreach (ParameterValueClass<SelectableItem> firstValue in ParameterValues)
            {
                if (firstValue.ParameterValue.IsSelected)
                {
                    if (firstValue.ParameterValue.ItemValue == value)
                        returnedValue = true; // zaznaczenie zgodne z warunkiem dla value
                    else
                        return false; // zaznaczenie nie zgodne więc można zwrócić źle (ret false)
                }
            }
            return returnedValue; // jeśli nic nie było zaznaczone to (ret false), jeśli cokolwiek było to wszystko spełniło warunek (ret true) 
        }
        public override bool NotEqualTo(string value)
        {
            bool returnedValue = true; // jeszcze nic nie zaznaczone (ret true)
            foreach (ParameterValueClass<SelectableItem> firstValue in ParameterValues)
            {
                if (firstValue.ParameterValue.IsSelected)
                {
                    if (firstValue.ParameterValue.ItemValue != value)
                        returnedValue = true; // zaznaczenie zgodne z warunkiem dla value
                    else
                        return false; // zaznaczenie nie zgodne więc można zwrócić źle (ret false)
                }
            }
            return returnedValue; // jeśli nic nie było zaznaczone to albo jeśli cokolwiek było to wszystko spełniło warunek (ret true) 
        }
        public override bool GreaterThan(string value)
        {
            throw new InapplicableConditionException("EnumerationParameter - GreaterThan - SecondArgument Value");
        }
        public override bool GreaterOrEqual(string value)
        {
            throw new InapplicableConditionException("EnumerationParameter - GreaterOrEqual - SecondArgument Value");
        }
        public override bool LessThan(string value)
        {
            throw new InapplicableConditionException("EnumerationParameter - LessThan - SecondArgument Value");
        }
        public override bool LessOrEqual(string value)
        {
            throw new InapplicableConditionException("EnumerationParameter - LessOrEqual - SecondArgument Value");
        }
        public override bool RegEx(string value)
        {
            throw new InapplicableConditionException("EnumerationParameter - RegEx - SecondArgument Value");
        }
        public override bool IsSubset(List<string> set)
        {
            bool returnedValue = false; // nic nie zaznaczone (ret false)
            foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
            {
                if (value.ParameterValue.IsSelected)
                {
                    returnedValue = false; // zaznaczone ale jeszcze nie wiadomo czy w zadanym zbiorze
                    if (set.Contains(value.ParameterValue.ItemValue))
                        returnedValue = true; // jest w zbiorze
                    else
                        return false; //nie jest w zbiorze więc cały warunek nie spełniony (ret false)
                }
            }
            return returnedValue; // nic nie zaznaczone (ret false) albo wszystko w zbiorze (ret true)
        }
        public override bool EmptyIntersection(List<string> set)
        {
            bool returnedValue = true; // nic nie zaznaczone (ret true)
            foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
            {
                if (value.ParameterValue.IsSelected)
                {
                    if (set.Contains(value.ParameterValue.ItemValue))
                        return false; // jest w zbiorze wiec warunek nie spełniony (ret false)
                }
            }
            return returnedValue; // nic nie zaznaczone albo wszystko poza zbiorem (ret true)
        }
        public override bool NotEmptyIntersection(List<string> set)
        {
            foreach (ParameterValueClass<SelectableItem> value in ParameterValues)
            {
                if (value.ParameterValue.IsSelected)
                {
                    if (set.Contains(value.ParameterValue.ItemValue))
                        return true;
                }
            }
            return false;
        }
        #endregion
    }

    public class SelectableItem
    {
        private bool _isSelected;
        private string _itemValue;

        public SelectableItem(bool isSelected, string itemValue)
        {
            this._isSelected = isSelected;
            this._itemValue = itemValue;
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public string ItemValue
        {
            get
            {
                return _itemValue;
            }
        }

    }
}
