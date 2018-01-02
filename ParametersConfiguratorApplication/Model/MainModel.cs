using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersConfiguratorApplication.Model
{
    public class MainModel
    {
        private ParametersConfigurator _parametersConfiguratorModel;
        
        private string _applicationTitle;
        private string _status;

        public MainModel()
        {
            this._parametersConfiguratorModel = null;
            this._applicationTitle = "Parameters Configurator";
            this._status = "Witaj";
            
        }

        public ParametersConfigurator ParametersConfiguratorModel
        {
            get
            {
                return _parametersConfiguratorModel;
            }
            set
            {
                _parametersConfiguratorModel = value;
            }
        }

        public string ApplicationTitle
        {
            get
            {
                return _applicationTitle;
            }
            set
            {
                _applicationTitle = value;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
    }
}
