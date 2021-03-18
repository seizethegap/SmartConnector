using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConnector.Services
{
    public sealed class ConsoleService : INotifyPropertyChanged
    { 
        public ConsoleService() { }

        private static ConsoleService instance;
        private static readonly object padlock = new object();

        private string commState, controlState;

        public static ConsoleService getInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConsoleService();
                    }
                    return instance;
                }
            }
        }

        private ObservableCollection<string> _ConsoleMessages = new ObservableCollection<string>();
        private ObservableCollection<string> _RestLogMessages = new ObservableCollection<string>();

        public ObservableCollection<string> ConsoleMessages
        {
            get
            {
                return _ConsoleMessages;
            }
            set
            {
                _ConsoleMessages = value;
                OnPropertyChanged("ConsoleMessages");
            }
        }

        public ObservableCollection<string> RestLogMessages
        {
            get
            {
                return _RestLogMessages;
            }
            set
            {
                _RestLogMessages = value;
                OnPropertyChanged("RestLogMessages");
            }
        }

        public string CommState
        {
            get
            {
                return commState;
            }
            set
            {
                switch (value)
                {
                    case "0":
                        commState = "Disabled";
                        break;
                    case "1":
                        commState = "Enabled/WaitDelay";
                        break;
                    case "2":
                        commState = "Enabled/WaitCRA";
                        break;
                    case "3":
                        commState = "Communicating";
                        break;
                    default:
                        commState = "Unknown communication state";
                        break;
                }
                OnPropertyChanged("CommState");
            }
        }

        public string ControlState
        {
            get
            {
                return controlState;
            }
            set
            {
                switch (value)
                {
                    case "1":
                        controlState = "OFFLINE Equipment Offline";
                        break;
                    case "2":
                        controlState = "OFFLINE Attempt Online";
                        break;
                    case "3":
                        controlState = "OFFLINE Host Offline";
                        break;
                    case "4":
                        controlState = "Local";
                        break;
                    case "5":
                        controlState = "Remote";
                        break;
                    default:
                        controlState = "Unknown control state";
                        break;
                }
                OnPropertyChanged("ControlState");
            }
        }

        public void WriteToConsole(string message)
        {
            ConsoleMessages.Add(DateTime.Now.ToString(@"[MM\/dd\/yyyy h\:mm tt] ") + message);
            OnPropertyChanged("ConsoleMessages");
        }

        public void WriteToRestLog(string message)
        {
            RestLogMessages.Add(DateTime.Now.ToString(@"[MM\/dd\/yyyy h\:mm tt] ") +  message);
            OnPropertyChanged("RestLogMessages");
        }

        public void ClearConsole()
        {
            ConsoleMessages.Clear();
        }

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
