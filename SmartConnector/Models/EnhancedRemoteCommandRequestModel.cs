using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConnector.Models
{
    public class EnhancedRemoteCommandRequestModel : INotifyPropertyChanged
    {
        private List<string> _LotIdList;
        private List<string> _PpIdList;
        string _State;
        string _Rcmd;
        string _SendId;

        public EnhancedRemoteCommandRequestModel()
        {
            _LotIdList = new List<string>();
            _PpIdList = new List<string>();
            _State = null;
            _Rcmd = null;
            _SendId = null;
        }

        #region Binding variables
        public List<string> LotIdList
        {
            get
            {
                return _LotIdList;
            }
            set
            {
                _LotIdList = value;
                OnPropertyChanged("LotIdList");
            }
        }

        public List<string> PpIdList
        {
            get
            {
                return _PpIdList;
            }
            set
            {
                _PpIdList = value;
                OnPropertyChanged("PpIdList");
            }
        }

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
                    OnPropertyChanged("State");
                }
            }
        }

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
