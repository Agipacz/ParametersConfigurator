using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ParametersConfiguratorApplication.Model
{
    public struct SignsOfSet
    {
        public string _beginningSign;
        public string _separatingSign;
        public string _endingSign;
    }

    public class ParametersConfigurator
    {
        private string _parametersConfiguratorTitle;
        private string _parametersConfiguratorIntroduction;

        private List<Section> _sections;
        private ConditionalSection _corrcectnessConditions;

        private string _parametersConfiguratorSummary;

        private bool _additionalParameters;

        public ParametersConfigurator(string parametersConfiguratorTitle, string parametersConfiguratorIntroduction, List<Section> sections, ConditionalSection correctnessConditions,
                                        string parametersConfiguratorSummary, bool additionalParameters)
        {
            this._parametersConfiguratorTitle = parametersConfiguratorTitle;
            this._parametersConfiguratorIntroduction = parametersConfiguratorIntroduction;
            this._sections = sections;
            this._corrcectnessConditions = correctnessConditions;
            this._parametersConfiguratorSummary = parametersConfiguratorSummary;
            this._additionalParameters = additionalParameters;
        }

        public string ParametersConfiguratorTitle
        {
            get
            {
                return _parametersConfiguratorTitle;
            }
            set
            {
                _parametersConfiguratorTitle = value;
            }
        }

        public string ParametersConfiguratorIntroduction
        {
            get
            {
                return _parametersConfiguratorIntroduction;
            }
            set
            {
                _parametersConfiguratorIntroduction = value;
            }
        }

        public List<Section> Sections
        {
            get
            {
                return _sections;
            }
        }

        public ConditionalSection CorrectnessConditions
        {
            get
            {
                return _corrcectnessConditions;
            }
        }

        public string ParametersConfiguratorSummary
        {
            get
            {
                return _parametersConfiguratorSummary;
            }
            set
            {
                _parametersConfiguratorSummary = value;
            }
        }

        public bool AdditionalParameters
        {
            get
            {
                return _additionalParameters;
            }
        }

        public void AddNewParameter(Parameter parameter)
        {
            Section lastSection;
            if (Sections.Count != 0)
            {
                lastSection = LastSection(Sections.Last());
            }
            else
            {
                lastSection = new Section(new ObservableCollection<Parameter>(), new TrueLogicalObject());
                Sections.Add(lastSection);
            }
            lastSection.Parameters.Add(parameter);
            
        }

        private Section LastSection(Section section)
        {
            if(section.Count() != 0 && section.Last().Sections.Count != 0)
                return LastSection(section.Last().Sections.Last());
            return section;
        }
    }
}
