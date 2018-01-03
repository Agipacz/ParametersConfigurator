using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersConfiguratorApplication.Model
{
    public enum TypeOfParameter : byte { Boolean, Integer, Float, Enumeration, Text, DateTime, Date };
    public enum ParameterUsageType : byte { Use, UseEmpty, Skip}

    public abstract class Parameter
    {
        protected string _name;
        protected string _label;
        protected string _description;

        protected SignsOfSet _signsOfSet;

        protected List<Section> _sections;

        protected bool _isOptional;
        protected int _minNumberOfValues;
        protected int _maxNumberOfValues;

        protected TypeOfParameter _typeOfParameter;

        protected ParameterUsageType _isUsed;

        protected bool _isVisible;

        protected string _bounds;

        protected bool _isValid;
        protected string _errorsStack;

        protected double _currentHeight;
        protected double _computedHegiht;

        public Parameter(string name, string label, string description, SignsOfSet signsOfSet, List<Section> sections, bool isOptional, int minNumberOfValues, int maxNumberOfValues, TypeOfParameter typeOfParameter)
        {
            this._name = name;
            this._label = label;
            this._description = description;
            this._signsOfSet = signsOfSet;
            this._sections = sections;
            this._isOptional = isOptional;
            this._minNumberOfValues = minNumberOfValues;
            if(minNumberOfValues > maxNumberOfValues)
                this._maxNumberOfValues = minNumberOfValues;
            else
                this._maxNumberOfValues = maxNumberOfValues;
            this._typeOfParameter = typeOfParameter;
            this._isUsed = isOptional ? ParameterUsageType.Skip : ParameterUsageType.Use;

            this._isVisible = true;
            this._isValid = true;

            this._bounds = null;
            this._errorsStack = null;
            
        }

        public abstract void AddParameterValue();
        public abstract void RemoveParameterValue<T>(ParameterValueClass<T> item);
        public abstract int ParameterValuesCount();
        public abstract bool IsParameterValuesButtonsVisible();
        public abstract string ComputeBounds();
        public abstract double ComputeHeight();
        public abstract string ToFileSaveTxt(ViewModel.SavingType savingType);

        public abstract bool EqualTo(Parameter parameter);
        public abstract bool NotEqualTo(Parameter parameter);
        public abstract bool GreaterThan(Parameter parameter);
        public abstract bool GreaterOrEqual(Parameter parameter);
        public abstract bool LessThan(Parameter parameter);
        public abstract bool LessOrEqual(Parameter parameter);
        public abstract bool IsSubset(Parameter parameter);
        public abstract bool EmptyIntersection(Parameter parameter);
        public abstract bool NotEmptyIntersection(Parameter parameter);
        public abstract bool EqualTo(string value);
        public abstract bool NotEqualTo(string value);
        public abstract bool GreaterThan(string value);
        public abstract bool GreaterOrEqual(string value);
        public abstract bool LessThan(string value);
        public abstract bool LessOrEqual(string value);
        public abstract bool RegEx(string value);
        public abstract bool IsSubset(List<string> set);
        public abstract bool EmptyIntersection(List<string> set);
        public abstract bool NotEmptyIntersection(List<string> set);


        public bool IsParameterValuesButtonsVisibleProperty
        {
            get
            {
                return IsParameterValuesButtonsVisible();
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string Label
        {
            get
            {
                if (_label != null)
                    return _label;
                else
                    return _name;
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
        }

        public SignsOfSet SignsOfSet
        {
            get
            {
                return _signsOfSet;
            }
        }

        public List<Section> Sections
        {
            get
            {
                return _sections;
            }
        }

        public bool IsOptional
        {
            get
            {
                return _isOptional;
            }
        }

        public int MinNumberOfValues
        {
            get
            {
                return _minNumberOfValues;
            }
        }

        public int MaxNumberOfValues
        {
            get
            {
                return _maxNumberOfValues;
            }
        }

        public string NumberOf
        {
            get
            {
                if (MinNumberOfValues == MaxNumberOfValues)
                    return MinNumberOfValues.ToString();
                else
                    return MinNumberOfValues.ToString() + " - " + MaxNumberOfValues.ToString();
            }
        }

        public TypeOfParameter TypeOfParameter
        {
            get
            {
                return _typeOfParameter;
            }
        }

        public ParameterUsageType IsUsed
        {
            get
            {
                return _isUsed;
            }
            set
            {
                _isUsed = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        public Parameter Self
        {
            get
            {
                return this;
            }
        }

        public string Bounds
        {
            get
            {
                if (_bounds == null)
                    _bounds = ComputeBounds();
                return _bounds;
            }
        }

        public string ErrorsStack
        {
            get
            {
                return _errorsStack;
            }
            set
            {
                _errorsStack = value;
            }
        }
        public double CurrentHeight
        {
            get
            {
                return _currentHeight;
            }
            set
            {
                if (_currentHeight != value)
                {
                    _currentHeight = value;
                    //Console.WriteLine("Current Height '"+ Label+ "':" + _currentHeight);
                }
            }
        }
        public double Height
        {
            get
            {
                return _computedHegiht;
            }
        }

        public override string ToString()
        {
            return Label;
        }

        public object SelectedItems { get; set; }

        public string SavingType(ViewModel.SavingType savingType)
        {
            switch (savingType)
            {
                case ViewModel.SavingType.Quotation:
                    return "\"";
                case ViewModel.SavingType.Apostrophe:
                    return "'";
                case ViewModel.SavingType.None:
                default:
                    return "";
            }
        }

    }
    public class ParameterValueClass<T>
    {
        private T _parameterValue;

        public ParameterValueClass()
        {
            _parameterValue = default(T);
        }

        public ParameterValueClass(T value)
        {
            _parameterValue = value;
        }

        public T ParameterValue
        {
            get
            {
                return _parameterValue;
            }
            set
            {
                 _parameterValue = value;
            }
        }

        public override string ToString()
        {
            if (_parameterValue != null)
                return _parameterValue.ToString();
            return null;
        }
    }
}
