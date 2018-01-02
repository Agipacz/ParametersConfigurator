using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ParametersConfiguratorApplication.ViewModel
{
    public class ParametersViewModel : BaseViewModel, IEnumerable<Model.Parameter>
    {
        private Model.MainModel _mainModel;

        private ObservableCollection<Model.Parameter> _allParameters;
        private ObservableCollection<Model.Parameter> _visibleParameters;

        private Model.Parameter _firstVisibleParameter;
        private Model.Parameter _nextFirstVisibleParameter;
        private bool _showNextPage;
        private bool _showPreviousPage;

        private bool _areAllParameresValid;

        private List<string> _existingParametersNames;

        private ICommand _showViewModelCommand;

        private ICommand _showFirstPageCommand;
        private ICommand _showNextPageCommand;
        private ICommand _showPreviousPageCommand;
        private ICommand _showLastPageCommand;

        private ICommand _refreshAllConditionsCommand;
        private ICommand _addParameterValueCommand;
        private ICommand _removeParameterValueCommand;
        private ICommand _addNewParameterCommand;

        private bool _checkCorrectnessConditions;

        private double _heightOfParametersGrid;

        public ParametersViewModel(Model.MainModel mainModel)
        {
            this._mainModel = mainModel;
            this._allParameters = new ObservableCollection<Model.Parameter>();
            this._visibleParameters = new ObservableCollection<Model.Parameter>();
            this._firstVisibleParameter = null;
            this._nextFirstVisibleParameter = null;
            this._areAllParameresValid = true;
            this._showNextPage = false;
            this._showPreviousPage = false;
            this._existingParametersNames = new List<string>();

            this._checkCorrectnessConditions = true;
        }

        public ObservableCollection<Model.Parameter> VisibleParameters
        {
            get
            {
                if (_mainModel.ParametersConfiguratorModel != null)
                {
                    if (_allParameters.Count == 0)
                    {
                        GatherAllParameters(_mainModel.ParametersConfiguratorModel.Sections);
                        RefreshCorrectnessConditions(_mainModel.ParametersConfiguratorModel.CorrectnessConditions);
                    }
                    if(_showNextPage)
                    {
                        _showNextPage = false;
                        // _lastFirstVisibleParameter = _firstVisibleParameter;
                        _firstVisibleParameter = _nextFirstVisibleParameter;
                    }
                    else if(_showPreviousPage)
                    {
                        _showPreviousPage = false;
                        double sumOfHeight = 0;
                        _visibleParameters.Clear();
                        foreach (Model.Parameter parameter in _allParameters)
                        {
                            if (parameter != _firstVisibleParameter)
                            {
                                if(parameter.IsVisible)
                                {
                                    sumOfHeight += parameter.Height;
                                    _visibleParameters.Add(parameter);
                                }
                            }
                            else
                                break;
                        }
                        List<Model.Parameter> parametersToRemove = new List<Model.Parameter>();
                        foreach(Model.Parameter parameter in _visibleParameters)
                        {
                            if(sumOfHeight < HeightOfParametersGrid)
                            {
                                _nextFirstVisibleParameter = _firstVisibleParameter;
                                _firstVisibleParameter = parameter;
                                break;
                            }
                            else
                            {
                                parametersToRemove.Add(parameter);
                                sumOfHeight -= parameter.Height;
                            }
                        }
                        foreach (Model.Parameter parameterToRemove in parametersToRemove)
                            _visibleParameters.Remove(parameterToRemove);
                        
                    }
                    if(!_showPreviousPage)
                    {
                        _visibleParameters.Clear();
                        _nextFirstVisibleParameter = null;
                        double remainHeight = HeightOfParametersGrid == 0 ? 430 : (HeightOfParametersGrid - 5);
                        bool startAddingToVisibleParameters = false;
                        foreach (Model.Parameter parameter in _allParameters)
                        {
                            if (parameter == _firstVisibleParameter && !startAddingToVisibleParameters)
                            {
                                startAddingToVisibleParameters = true;
                            }
                            if (startAddingToVisibleParameters)
                            {
                                if (parameter.IsVisible)
                                {
                                    remainHeight -= parameter.Height;
                                    if (remainHeight > 0)
                                        _visibleParameters.Add(parameter);
                                    else
                                    {
                                        _nextFirstVisibleParameter = parameter;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                }
                return _visibleParameters;
            }
        }
        public bool AdditionalParameters
        {
            get
            {
                return _mainModel.ParametersConfiguratorModel.AdditionalParameters;
            }
        }
        public bool CheckCorrectnessConditions
        {
            get
            {
                return _checkCorrectnessConditions;
            }
            set
            {
                _checkCorrectnessConditions = value;
            }
        }
        public double HeightOfParametersGrid
        {
            get
            {
                return _heightOfParametersGrid;
            }
            set
            {
                if (_heightOfParametersGrid != value)
                {
                    _heightOfParametersGrid = value;
                    //MessageBox.Show("Wysokość okna parametrów: " + _heightOfParametersGrid);
                    Console.WriteLine("Wysokość okna parametrów: " + _heightOfParametersGrid);
                    OnPropertyChanged("VisibleParameters");
                }
                    
            }
        }
        public bool IsSummaryAvailable
        {
            get
            {
                if (_nextFirstVisibleParameter == null && _areAllParameresValid && _checkCorrectnessConditions)
                //if (_nextFirstVisibleParameter == null )
                        return true;
                return false;
            }
        }
        private void GatherAllParameters(List<Model.Section> sections)
        {
            foreach (Model.Section section in sections)
            {
                foreach (Model.Parameter parameter in section)
                {
                    _allParameters.Add(parameter);
                    _existingParametersNames.Add(parameter.Name);
                    if (parameter.Sections.Count != 0)
                        GatherAllParameters(parameter.Sections);
                }
            }
            if(_allParameters.Count != 0)
            {
                _firstVisibleParameter = _allParameters[0];
               // _nextFirstVisibleParameter = _allParameters[0];
                //_lastParameter = _allParameters.Last();
            }
        }
        public ICommand ShowFirstPageCommand
        {
            get
            {
                if (_showFirstPageCommand == null)
                {
                    _showFirstPageCommand = new RelayCommand(
                        p =>
                        {
                            _firstVisibleParameter = _allParameters[0];
                            _showNextPage = false;
                            _showPreviousPage = false;
                            OnPropertyChanged("VisibleParameters", "IsSummaryAvailable");
                        },
                        p =>
                        {
                            if (_allParameters.Count != 0)
                                return _firstVisibleParameter != _allParameters[0];
                            return false;
                        }
                        );
                }
                return _showFirstPageCommand;
            }
        }
        public ICommand ShowPreviousPageCommand
        {
            get
            {
                if (_showPreviousPageCommand == null)
                {
                    _showPreviousPageCommand = new RelayCommand(
                        p =>
                        {
                            _showPreviousPage = true;
                            OnPropertyChanged("VisibleParameters", "IsSummaryAvailable");
                        },
                        p =>
                        {
                            if(_allParameters.Count != 0)
                                return _firstVisibleParameter != _allParameters[0];
                            return false;
                        }
                        );
                }
                return _showPreviousPageCommand;
            }
        }
        public ICommand ShowNextPageCommand
        {
            get
            {
                if (_showNextPageCommand == null)
                {
                    _showNextPageCommand = new RelayCommand(
                        p =>
                        {
                            _showNextPage = true;
                            OnPropertyChanged("VisibleParameters", "IsSummaryAvailable");
                        },
                        p => (_nextFirstVisibleParameter != null)
                        );
                }
                return _showNextPageCommand;
            }
        }
        public ICommand ShowLastPageCommand
        {
            get
            {
                if (_showLastPageCommand == null)
                {
                    _showLastPageCommand = new RelayCommand(
                        p =>
                        {
                            _firstVisibleParameter = null;
                            _showPreviousPage = true;
                            OnPropertyChanged("VisibleParameters", "IsSummaryAvailable");
                        },
                        p => (_nextFirstVisibleParameter != null)
                        );
                }
                return _showLastPageCommand;
            }
        }
        public ICommand AddParameterValueCommand
        {
            get
            {
                if (_addParameterValueCommand == null)
                {
                    _addParameterValueCommand = new RelayCommand(
                        p =>
                        {
                            AddParameterValue((Model.Parameter)p);
                            RefreshCorrectnessConditions(_mainModel.ParametersConfiguratorModel.CorrectnessConditions);
                            OnPropertyChanged("ParameterValues", "SelectedItems", "Height", "VisibleParameters");
                        },
                        p =>
                        {
                            if (p != null && p != DependencyProperty.UnsetValue)
                            {
                                if (((Model.Parameter)p).MaxNumberOfValues > ((Model.Parameter)p).ParameterValuesCount())
                                    return true;
                                return false;
                            }
                            return false;
                        }
                        );
                }
                return _addParameterValueCommand;
            }
        }
        private void AddParameterValue(Model.Parameter parameter)
        {
            parameter.AddParameterValue();
            double newHeight = 0;
            foreach(Model.Parameter visibleParameter in _visibleParameters)
            {
                newHeight += visibleParameter.Height;
            }
        }
        public ICommand RemoveParameterValueCommand
        {
            get
            {
                if (_removeParameterValueCommand == null)
                {
                    _removeParameterValueCommand = new RelayCommand(
                        p =>
                        {
                            RemoveParameterValue((ParameterItemPair)p);
                            RefreshVisibilityOfParameters(_mainModel.ParametersConfiguratorModel.Sections, true);
                            RefreshCorrectnessConditions(_mainModel.ParametersConfiguratorModel.CorrectnessConditions);
                            OnPropertyChanged("ParameterValues", "SelectedItems", "VisibleParameters");
                        },
                        p =>
                        {
                            if (p != null)
                            {
                                if (((ParameterItemPair)p)._parameter.MinNumberOfValues < ((ParameterItemPair)p)._parameter.ParameterValuesCount())
                                    return true;
                                return false;
                            }
                            return false;
                        }

                        );
                }
                return _removeParameterValueCommand;
            }
        } 
        private void RemoveParameterValue(ParameterItemPair pair)
        {

            switch (pair._parameter.TypeOfParameter)
            {
                case Model.TypeOfParameter.Boolean:
                    {
                        (pair._parameter as Model.BooleanParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<bool>);
                        break;
                    }
                case Model.TypeOfParameter.Date:
                    {
                        (pair._parameter as Model.DateParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<DateTime>);
                        break;
                    }
                case Model.TypeOfParameter.DateTime:
                    {
                        (pair._parameter as Model.DateTimeParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<DateTime>);
                        break;
                    }
                case Model.TypeOfParameter.Enumeration:
                    {
                        (pair._parameter as Model.EnumerationParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<string>);
                        break;
                    }
                case Model.TypeOfParameter.Float:
                    {
                        (pair._parameter as Model.FloatParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<double>);
                        break;
                    }
                case Model.TypeOfParameter.Integer:
                    {
                        (pair._parameter as Model.IntegerParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<int>);
                        break;
                    }
                case Model.TypeOfParameter.Text:
                    {
                        (pair._parameter as Model.TextParameter).RemoveParameterValue(pair._item as Model.ParameterValueClass<string>);
                        break;
                    }
            }
            
        }
        public IEnumerator<Model.Parameter> GetEnumerator()
        {
            return _visibleParameters.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
        public ICommand ShowViewModelCommand
        {
            get
            {
                if (_showViewModelCommand == null)
                {
                    _showViewModelCommand = new RelayCommand(
                        p =>
                        {
                            string s = "VM: ";
                            foreach (Model.Parameter param in _visibleParameters)
                            {
                                s += param.ToString();
                            }
                            s += "\nMO: ";
                            foreach (Model.Section section in _mainModel.ParametersConfiguratorModel.Sections)
                            {
                                foreach (Model.Parameter parameter in section)
                                {
                                    s += parameter.ToString();
                                }
                            }
                            MessageBox.Show(s);
                        },
                        p => true
                        );
                }
                return _showViewModelCommand;
            }
        }
        public ICommand AddNewParameterCommand
        {
            get
            {
                if(_addNewParameterCommand == null)
                {
                    _addNewParameterCommand = new RelayCommand(
                        p =>
                        {
                            Model.TextParameter newParameter = new Model.TextParameter(((string)p).Trim(), default(string), default(string), new Model.SignsOfSet(), new List<Model.Section>(), true, 1, 1, default(string));
                            _mainModel.ParametersConfiguratorModel.AddNewParameter(newParameter);
                            _allParameters.Clear();
                            GatherAllParameters(_mainModel.ParametersConfiguratorModel.Sections);
                            OnPropertyChanged("VisibleParameters");
                        },
                        p => !_existingParametersNames.Contains(((string)p).Trim()) && ((string)p).Trim() != ""
                        );
                }
                return _addNewParameterCommand;
            }
        }
        public ICommand RefreshAllConditionsCommand
        {
            get
            {
                if (_refreshAllConditionsCommand == null)
                {
                    _refreshAllConditionsCommand = new RelayCommand(
                        p =>
                        {
                            RefreshVisibilityOfParameters(_mainModel.ParametersConfiguratorModel.Sections, true);
                            RefreshCorrectnessConditions(_mainModel.ParametersConfiguratorModel.CorrectnessConditions);
                            OnPropertyChanged("ErrorsStack", "IsValid", "Height", "CurrentHeight", "IsSummaryAvailable", "VisibleParameters", "IsMaxSelected");
                        },
                        p => true
                        );
                }
                return _refreshAllConditionsCommand;
            }
        }
        private void RefreshVisibilityOfParameters(List<Model.Section> sections, bool isHigherSectionVisible)
        {
            foreach (Model.Section section in sections)
            {
                foreach (Model.Parameter parameter in section)
                {
                    if (isHigherSectionVisible)
                    {
                        parameter.IsVisible = section.IsSectionVisible;
                        if (parameter.Sections.Count != 0)
                            RefreshVisibilityOfParameters(parameter.Sections, parameter.IsVisible);
                    }
                    else
                    {
                        parameter.IsVisible = false;
                        if (parameter.Sections.Count != 0)
                            RefreshVisibilityOfParameters(parameter.Sections, false);
                    }
                }
            }
        }
        private void RefreshCorrectnessConditions(Model.LogicalObject logicalObject)
        {
            if(_checkCorrectnessConditions)
            {
                CleanAllErrosStacks(_allParameters);
                if (logicalObject != null)
                {
                    Model.ConditionalSection mainConditionalSection = logicalObject as Model.ConditionalSection;
                    Model.LogicalObject errorSection = mainConditionalSection.ComputeErrorsStacks(mainConditionalSection.LogicalConnector);
                    if (!errorSection.LogicalValue)
                    {
                        SetNewErrors(errorSection as Model.ConditionalSection);
                        _areAllParameresValid = false;
                    }
                    else
                        _areAllParameresValid = true;
                }
            }
        }
        private void CleanAllErrosStacks(ObservableCollection<Model.Parameter> allParameters)
        {
            foreach (Model.Parameter parameter in allParameters)
            {
                parameter.IsValid = true;
                parameter.ErrorsStack = "";
            }
        }
        private void SetNewErrors(Model.ConditionalSection section)
        {
            HashSet<Model.Parameter> parametersWithErrors = ListParametersFromCorrectnessConditions(section);
            if (parametersWithErrors.Count != 0)
            {
                foreach (Model.Parameter parameter in parametersWithErrors)
                {
                    string errorsString = "";
                    errorsString = BuildErrorsString(errorsString, parameter, section, section.LogicalConnector == Model.LogicalConnector.AND ? false : true, "");
                    parameter.ErrorsStack = errorsString.Trim();
                    parameter.IsValid = false;
                }
            }          
        }
        private static string BuildErrorsString(string errorString, Model.Parameter parameter, Model.ConditionalSection section, bool modeOR, string space)
        {
            string moreSpace = "    ";
            if (modeOR)
            {
                switch (section.LogicalConnector)
                {
                    case Model.LogicalConnector.AND:
                        {
                            string errors = "";
                            foreach (Model.LogicalObject logicObject in section.Subsections)
                            {
                                Model.Condition condition = logicObject as Model.Condition;
                                if (condition != null)
                                {
                                    errors += "\n" + space + condition.ToString();
                                    condition.FirstParameter.IsValid = false;
                                }
                                else
                                {
                                    Model.ConditionalSection insideSection = logicObject as Model.ConditionalSection;
                                    if (insideSection != null)
                                        errors = BuildErrorsString(errors, parameter, insideSection, false, space + moreSpace);
                                }
                            }
                            if (errors != "")
                                errorString += "\n"+ space + "( Sekcja AND" + errors + ")";
                            return errorString;
                        }
                    case Model.LogicalConnector.OR:
                        {
                            string errors = "";
                            
                            foreach (Model.LogicalObject logicObject in section.Subsections)
                            {
                                Model.Condition condition = logicObject as Model.Condition;
                                if (condition != null)
                                {
                                    errors += "\n" + space + condition.ToString();
                                }
                                else
                                {
                                    Model.ConditionalSection insideSection = logicObject as Model.ConditionalSection;
                                    if(insideSection != null)
                                        errors = BuildErrorsString(errors, parameter, insideSection, false, space + moreSpace);
                                }
                            }
                            if (errors != "")
                                errorString += "\n" + space + "( Sekcja OR" + errors + ")";
                            return errorString;
                        }
                    default:
                        throw new Exception("Łącznik logiczny " + section.LogicalConnector + " nie jest obsługiwany.");
                }
            }
        
            else
            {
                switch (section.LogicalConnector)
                {
                    case Model.LogicalConnector.AND:
                        {
                            string errors = "";
                            foreach (Model.LogicalObject logicObject in section.Subsections)
                            {
                                Model.Condition condition = logicObject as Model.Condition;
                                if (condition != null)
                                {
                                    if (condition.FirstParameter == parameter)
                                    {
                                        errors += "\n" + space + condition.ToString();
                                        condition.FirstParameter.IsValid = false;
                                    }
                                    
                                }
                                else
                                {
                                    Model.ConditionalSection insideSection = logicObject as Model.ConditionalSection;
                                    if(insideSection != null)
                                        errors = BuildErrorsString(errors, parameter, insideSection, false, space + moreSpace);
                                }
                            }
                            if(errors != "")
                                errorString += "\n" + space + "( Sekcja AND" + errors + ")";
                            return errorString;
                        }
                    case Model.LogicalConnector.OR:
                        {
                            string errors = "";
                            bool isRelevant = false;
                            foreach (Model.LogicalObject logicObject in section.Subsections)
                            {
                                Model.Condition condition = logicObject as Model.Condition;
                                if (condition != null)
                                {
                                    if (condition.FirstParameter == parameter)
                                        isRelevant = true;
                                    errors += "\n" + space + condition.ToString();
                                }
                                else
                                {
                                    Model.ConditionalSection insideSection = logicObject as Model.ConditionalSection;
                                    if (insideSection != null)
                                        errors = BuildErrorsString(errors, parameter, insideSection, false, space + moreSpace);
                                }
                            }
                            if(isRelevant && errors != "")
                                errorString += "\n" + space + "( Sekcja OR" + errors + ")";
                            return errorString;
                        }
                    default:
                        throw new Exception("Łącznik logiczny " + section.LogicalConnector + " nie jest obsługiwany.");
                }
            }
        }
        private static HashSet<Model.Parameter> ListParametersFromCorrectnessConditions(Model.ConditionalSection section)
        {
            HashSet<Model.Parameter> parameters = new HashSet<Model.Parameter>();
            foreach (Model.LogicalObject logicalObject in section.Subsections)
            {
                if (logicalObject != null)
                {
                    Model.Condition contidion = logicalObject as Model.Condition;
                    if (contidion != null)
                    {
                        if (contidion.FirstParameter.IsUsed == Model.ParameterUsageType.Use)
                            parameters.Add(contidion.FirstParameter);
                    }
                    else
                    {
                        Model.ConditionalSection nextSection = logicalObject as Model.ConditionalSection;
                        if (nextSection != null)
                            parameters.UnionWith(ListParametersFromCorrectnessConditions(nextSection));
                    }
                }
            }
            return parameters;
        }
        private static List<Model.Parameter> ListParameters(List<Model.Section> sections)
        {
            List<Model.Parameter> parameters = new List<Model.Parameter>();
            foreach (Model.Section section in sections)
            {
                foreach (Model.Parameter parameter in section)
                {
                    parameters.Add(parameter);
                    if (parameter.Sections.Count != 0)
                        parameters.AddRange(ListParameters(parameter.Sections));
                }
            }
            return parameters;
        }
    }

    public struct ParameterItemPair
    {
        public Model.Parameter _parameter;
        public object _item;
    }

}
