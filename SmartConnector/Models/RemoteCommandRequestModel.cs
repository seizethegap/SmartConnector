using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConnector.Models
{
    public class RemoteCommandRequestModel : INotifyPropertyChanged
    {
        private string _Rcmd;
        private string _SendId;

        public RemoteCommandRequestModel()
        {
            _Rcmd = null;
            _SendId = null;
        }

        #region Binding variables
        public string Rcmd
        {
            get
            {
                return _Rcmd;
            }
            set
            {
                if (_Rcmd != value)
                {
                    _Rcmd = value;
                    OnPropertyChanged("Rcmd");
                }
            }
        }

        public string SendId
        {
            get
            {
                return _SendId;
            }
            set
            {
                if (_SendId != value)
                {
                    _SendId = value;
                    OnPropertyChanged("SendId");
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
