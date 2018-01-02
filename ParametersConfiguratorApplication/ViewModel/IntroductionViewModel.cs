using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersConfiguratorApplication.ViewModel
{
    public class IntroductionViewModel : BaseViewModel
    {
        private Model.MainModel _mainModel;

        public IntroductionViewModel(Model.MainModel mainModel)
        {
            this._mainModel = mainModel;
        }

        public string IntroductionContent
        {
            get
            {
                if (_mainModel.ParametersConfiguratorModel != null)
                    return _mainModel.ParametersConfiguratorModel.ParametersConfiguratorIntroduction;
                return null;
            }
        }
    }

    
}
