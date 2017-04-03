using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CN2.UC.ArguedLearningExamplesView
{
    public class PredictiveAttributeValue//:INotifyPropertyChanged
    {
        private bool _isBecauseExpression;
        private bool _isDespiteExpression;

        public string Value { get; set; }

        public bool IsBecauseExpression
        {
            get { return _isBecauseExpression; }
            set
            {
                _isBecauseExpression = value;
                _isDespiteExpression = !value && _isDespiteExpression;
                NotifyPropertyChanged("IsBecauseExpression");
                NotifyPropertyChanged("IsBecauseExpression");
            }
        }

        public bool IsDespiteExpression
        {
            get { return _isDespiteExpression; }
            set
            {
                _isBecauseExpression = !value && _isBecauseExpression;
                _isDespiteExpression = value;
                NotifyPropertyChanged("IsDespiteExpression");
                NotifyPropertyChanged("IsDespiteExpression");
            }
        }

        public PredictiveAttributeValue()
        {
            Value = string.Empty;
            _isBecauseExpression = true;
            _isDespiteExpression = true;
        }

        public PredictiveAttributeValue(string value, bool isBecauseExpression, bool isDespiteExpression)
        {
            Value = value;
            _isBecauseExpression = isBecauseExpression;
            _isDespiteExpression = isDespiteExpression;
        }

        public PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
