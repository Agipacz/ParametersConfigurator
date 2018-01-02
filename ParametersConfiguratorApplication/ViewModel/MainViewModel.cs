using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ParametersConfiguratorApplication.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Model.MainModel mainModel;

        private string _validationInformation = "Wybierz plik XML do walidacji.";

        private ICommand _setParametersConfiguratorModelCommand;

        public MainViewModel()
        {
            mainModel = new Model.MainModel();
        }

        public Model.MainModel MainModel
        {
            get
            {
                return mainModel;
            }
        }

        public string ParametersConfiguratorTitle
        {
            get
            {
                if (mainModel.ParametersConfiguratorModel != null)
                    return mainModel.ParametersConfiguratorModel.ParametersConfiguratorTitle;
                return null;
            }
        }

        public string ValidationInformation
        {
            get
            {
                return _validationInformation;
            }
            set
            {
                _validationInformation = value;
            }
        }

        public ICommand SetParametersConfiguratorModelCommand
        {
            get
            {
                if (_setParametersConfiguratorModelCommand == null)
                {
                    _setParametersConfiguratorModelCommand = new RelayCommand(
                        p =>
                        {
                            try
                            {
                                XDocument xml = Model.XMLFile.ValidateXMLFile((string)p);
                                Model.ParametersConfigurator pc = Model.XMLFile.Load(xml);
                                ValidationInformation = "Walidacja przebiegła pomyślnie.";
                                mainModel.Status = "Wczytano plik";
                                mainModel.ParametersConfiguratorModel = pc;

                                OnPropertyChanged("ParametersConfiguratorTitle", "Status", "ValidationInformation");
                            }
                            catch (Exception ex)
                            {
                                ValidationInformation = ex.Message;
                                OnPropertyChanged("ValidationInformation");
                            }
                            
                        },
                        p => System.IO.File.Exists((string)p));
                }
                return _setParametersConfiguratorModelCommand;
            }
        }
    }

}
