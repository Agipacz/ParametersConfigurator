using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ParametersConfiguratorApplication.ViewModel
{
    public class ApplicationViewModel : BaseViewModel
    {
        #region Fields

        private string _applicationTitle;
        private ICommand _changeApplicationTitleCommand;

        private ICommand _startParametersConfiguratorCommand;
        private ICommand _changePageCommand;
        private ICommand _backToParametersCommand;
        private ICommand _showSummaryPageCommand;

        private BaseViewModel _currentPageViewModel;
        private List<BaseViewModel> _pageViewModels;

        private string _status;
        private ICommand _changeStatusCommand;

        #endregion

        public ApplicationViewModel()
        {
            PageViewModels.Add(new MainViewModel());
            PageViewModels.Add(new IntroductionViewModel(((MainViewModel)PageViewModels[0]).MainModel));
            PageViewModels.Add(new ParametersViewModel(((MainViewModel)PageViewModels[0]).MainModel));
            PageViewModels.Add(new SummaryViewModel(((MainViewModel)PageViewModels[0]).MainModel));

            CurrentPageViewModel = PageViewModels[0];
        }

        public string ApplicationTitle
        {
            get
            {
                if(_applicationTitle == null)
                {
                    _applicationTitle = ((MainViewModel)PageViewModels[0]).MainModel.ApplicationTitle;
                }
                return _applicationTitle;
            }
            set
            {
                if(_applicationTitle != value)
                {
                    _applicationTitle = value;
                    ((MainViewModel)PageViewModels[0]).MainModel.ApplicationTitle = value;
                    OnPropertyChanged("ApplicationTitle");
                }
            }
        }

        public string Status
        {
            get
            {
                return ((MainViewModel)PageViewModels[0]).MainModel.Status;
            }
            set
            {
                if(_status != value)
                {
                    _status = value;
                    ((MainViewModel)PageViewModels[0]).MainModel.Status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public List<BaseViewModel> PageViewModels
        {
            get
            {
                if(_pageViewModels == null)
                {
                    _pageViewModels = new List<BaseViewModel>();
                }
                return _pageViewModels;
            }
        }

        public BaseViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if(_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public ICommand ChangeApplicationTitleCommand
        {
            get
            {
                if(_changeApplicationTitleCommand == null)
                {
                    _changeApplicationTitleCommand = new RelayCommand(
                        p => ApplicationTitle = ((string)p),
                        p => p is string);
                }
                return _changeApplicationTitleCommand;
            }
        }

        public ICommand ChangeStatusCommand
        {
            get
            {
                if(_changeStatusCommand == null)
                {
                    _changeStatusCommand = new RelayCommand(
                        p => Status = ((string)p),
                        p => p is string);
                }
                return _changeStatusCommand;
            }
        }

        public ICommand StartParametersConfiguratorCommand
        {
            get
            {
                if(_startParametersConfiguratorCommand == null)
                {
                    _startParametersConfiguratorCommand = new RelayCommand(
                        p =>
                        {
                            if (((MainViewModel)PageViewModels[0]).MainModel.ParametersConfiguratorModel.ParametersConfiguratorIntroduction != null)
                            {
                                ChangeViewModel("IntroductionViewModel");
                            }
                            else
                            {
                                ChangeViewModel("ParametersViewModel");
                            }
                        },
                        p => ((MainViewModel)PageViewModels[0]).MainModel.ParametersConfiguratorModel is Model.ParametersConfigurator
                        );
                }
                return _startParametersConfiguratorCommand;
            }
        }
        public ICommand BackToParametersCommand
        {
             get
            {
                if (_backToParametersCommand == null)
                {
                    _backToParametersCommand = new RelayCommand(
                        p =>
                        {
                             ChangeViewModel("ParametersViewModel");
                        },
                        p => ((MainViewModel)PageViewModels[0]).MainModel.ParametersConfiguratorModel is Model.ParametersConfigurator
                        );
                }
                return _backToParametersCommand;
            }
        }
        public ICommand ShowSummaryPageCommand
        {
            get
            {
                if (_showSummaryPageCommand == null)
                {
                    _showSummaryPageCommand = new RelayCommand(
                        p => ChangeViewModel("SummaryViewModel"),
                        p => 
                        {
                            if (p != null)
                                return (bool)p;
                            return false;
                        }
                        );
                }
                return _showSummaryPageCommand;
            }
        }
        public ICommand ChangePageCommand
        {
            get
            {
                if(_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((string)p),
                        p => p is string
                        );
                }
                return _changePageCommand;
            }
        }

        private void ChangeViewModel(string viewModelClassName)
        {
            switch (viewModelClassName)
            {
                case "MainViewModel":
                    {
                        CurrentPageViewModel = PageViewModels[0];
                        break;
                    }
                case "IntroductionViewModel":
                    {
                        CurrentPageViewModel = PageViewModels[1];
                        break;
                    }
                case "ParametersViewModel":
                    {
                        CurrentPageViewModel = PageViewModels[2];
                        break;
                    }
                case "SummaryViewModel":
                    {
                        CurrentPageViewModel = PageViewModels[3];
                        break;
                    }

            }
        }
    }
}
