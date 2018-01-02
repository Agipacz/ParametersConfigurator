using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ParametersConfiguratorApplication.Model
{
    public class Section : IEnumerable<Parameter>
    {
        private ObservableCollection<Parameter> _parameters;
        private LogicalObject _visibilityCondition;

   
        public Section(ObservableCollection<Parameter> parameters, LogicalObject visibilityCondition)
        {
            this._parameters = parameters;
            this._visibilityCondition = visibilityCondition;
        }

        public LogicalObject VisibilityCondition
        {
            get
            {
                return _visibilityCondition;
            }
            set
            {
                _visibilityCondition = value;
            }
        }

        public bool IsSectionVisible
        {
            get
            {
                return ComputeSectionVisibility();
            }
        }

        public ObservableCollection<Parameter> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public bool ComputeSectionVisibility()
        {
            return _visibilityCondition.LogicalValue;
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
        
    }
}
