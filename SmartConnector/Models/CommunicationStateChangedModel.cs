using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartConnector.RESTServices;

namespace SmartConnector.Models
{
    public class CommunicationStateChangedModel : INotifyPropertyChanged
    {
        private string _State;

        public CommunicationStateChangedModel()
        {
            _State = null;
        }

        #region Binding variables
        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    OnPropertyChanged("CommnicationStateChanged");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
