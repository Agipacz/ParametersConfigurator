using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ParametersConfiguratorApplication.ViewModel
{
    public enum SavingType : byte { None, Quotation, Apostrophe}
    public class SummaryViewModel : BaseViewModel
    {
        private Model.MainModel _mainModel;

        private ICommand _saveParametersConfiguratorFileCommand;
        private ICommand _savingSuccessCommand;
        private ICommand _closeApplicationCommand;

        private SavingType _savingType;

        public SummaryViewModel(Model.MainModel mainModel)
        {
            this._mainModel = mainModel;
            _savingType = SavingType.None;
        }

        public string ApplicationTitle
        {
            get
            {
                return _mainModel.ApplicationTitle;
            }
        }

        public string SummaryContent
        {
            get
            {
                if (_mainModel != null)
                    return _mainModel.ParametersConfiguratorModel.ParametersConfiguratorSummary;
                return null;
            }
        }
        public SavingType SavingType
        {
            get
            {
                 return _savingType;
            }
            set
            {
                _savingType = value;
                OnPropertyChanged("SavingType");
            }
        }

        public ICommand SaveParametersConfiguratorFileCommand
        {
            get
            {
                if(_saveParametersConfiguratorFileCommand == null)
                {
                    _saveParametersConfiguratorFileCommand = new RelayCommand(
                        p =>
                        {
                            Model.XMLFile.Save((string)p, _mainModel.ParametersConfiguratorModel, SavingType);
                            _mainModel.Status = "Zapisano plik";
                            OnPropertyChanged("Status"); 
                        }
                        );
                }
                return _saveParametersConfiguratorFileCommand;
            }
        }
        public ICommand SavingSuccessCommand
        {
            get
            {
                if (_savingSuccessCommand == null)
                {
                    _savingSuccessCommand = new RelayCommand(
                        p =>
                        {
                            MessageBox.Show("Zapis do pliku udany!", _mainModel.ApplicationTitle);
                        },
                        p => true
                        );
                }
                return _savingSuccessCommand;
            }
        }
        public ICommand CloseApplicationCommand
        {
            get
            {
                if (_closeApplicationCommand == null)
                {
                    _closeApplicationCommand = new RelayCommand(
                        p =>
                        {
                            Application.Current.Shutdown();
                        },
                        p => true
                        );
                }
                return _closeApplicationCommand;
            }
        }
    }
}
