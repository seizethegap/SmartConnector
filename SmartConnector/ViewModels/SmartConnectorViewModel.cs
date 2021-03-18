using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using SmartConnector.Models;
using SmartConnector.RESTServices;
using SmartConnector.Services;
using SmartConnector.Commands;

namespace SmartConnector.ViewModels
{
    public class SmartConnectorViewModel : INotifyPropertyChanged
    {
        public SmartConnectorViewModel()
        {
            _ConsoleMessages = ConsoleService.getInstance.ConsoleMessages;

            ClearAlarmCommand = new ClearAlarmCommand(this);
            DisableCommunicationCommand = new DisableCommunicationCommand(this);
            EnableCommunicationCommand = new EnableCommunicationCommand(this);
            LocalControlCommand = new LocalControlCommand(this);
            OfflineControlCommand = new OfflineControlCommand(this);
            OnlineControlCommand = new OnlineControlCommand(this);
            RemoteControlCommand = new RemoteControlCommand(this);
            SendEventCommand = new SendEventCommand(this);
            SetAlarmCommand = new SetAlarmCommand(this);

            // subscribe property changed 
            ConsoleService.getInstance.PropertyChanged += SmartConnector_PropertyChanged;

            InitializeSmartConnector();
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private AlarmStateChangedModel AlarmStateChanged;
        private CommunicationStateChangedModel CommunicationStateChanged;
        private ControlStateChangedModel ControlStateChanged;
        private EnhancedRemoteCommandRequestModel EnhancedRemoteCommandRequest;
        private OperatorCommunicationStateReplyModel OperatorCommunicationStateReply;
        private OperatorControlStateReplyModel OperatorControlStateReply;
        private RemoteCommandRequestModel RemoteCommandRequest;

        private string _CommunicationState;
        private string _ControlState;
        private string _EventName, _EventDvName, _EventDvValue;
        private string _AlarmName, _AlarmId, _AlarmText;

        private ObservableCollection<string> _ConsoleMessages;
        private ObservableCollection<string> _RestLogMessages;

        #region Binding variables
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

        public List<string> LotIdList
        {
            get
            {
                return EnhancedRemoteCommandRequest.LotIdList;
            }
            set
            {
                EnhancedRemoteCommandRequest.LotIdList = value;
                OnPropertyChanged("LotIdList");
            }
        }

        public List<string> PpIdList
        {
            get
            {
                return EnhancedRemoteCommandRequest.PpIdList;
            }
            set
            {
                EnhancedRemoteCommandRequest.PpIdList = value;
                OnPropertyChanged("PpIdList");
            }
        }

        public string CommunicationState
        {
            get
            {
                return _CommunicationState;
            }
            set
            {
                if (_CommunicationState != value)
                {
                    _CommunicationState = value;
                    OnPropertyChanged("CommunicationState");
                }
            }
        }

        public string ControlState
        {
            get
            {
                return _ControlState;
            }
            set
            {
                if (_ControlState != value)
                {
                    _ControlState = value;
                    OnPropertyChanged("ControlState");
                }
            }
        }

        public string EventName
        {
            get
            {
                return _EventName;
            }
            set
            {
                _EventName = value;
            }
        }

        public string EventDvName
        {
            get
            {
                return _EventDvName;
            }
            set
            {
                _EventDvName = value;
            }
        }

        public string EventDvValue
        {
            get
            {
                return _EventDvValue;
            }
            set
            {
                _EventDvValue = value;
            }
        }

        public string AlarmName
        {
            get
            {
                return _AlarmName;
            }
            set
            {
                _AlarmName = value;
            }
        }

        public string AlarmId
        {
            get
            {
                return _AlarmId;
            }
            set
            {
                _AlarmId = value;
            }
        }

        public string AlarmText
        {
            get
            {
                return _AlarmText;
            }
            set
            {
                _AlarmText = value;
            }
        }
        #endregion

        #region Commands
        public ICommand EnableCommunicationCommand
        {
            get;
            private set;
        }

        public ICommand DisableCommunicationCommand
        {
            get;
            private set;
        }

        public ICommand OfflineControlCommand
        {
            get;
            private set;
        }

        public ICommand OnlineControlCommand
        {
            get;
            private set;
        }

        public ICommand RemoteControlCommand
        {
            get;
            private set;
        }

        public ICommand LocalControlCommand
        {
            get;
            private set;
        }

        public ICommand SendEventCommand
        {
            get;
            private set;
        }

        public ICommand SetAlarmCommand
        {
            get;
            private set;
        }

        public ICommand ClearAlarmCommand
        {
            get;
            private set;
        }
        #endregion

        public void InitializeSmartConnector(params string[] args)
        {
            ConsoleService.getInstance.WriteToConsole("Starting Smart Connector...");
            string serverPort = "8088";
            string serverAddress = "http://127.0.0.1:";
            string clientPort = "8080";
            string clientAddress = "http://127.0.0.1:";

            if (args.Length == 4)
            {
                clientAddress = args[0];
                clientPort = args[1];
                serverAddress = args[2];
                serverPort = args[3];
            }

            RESTManager.getInstance.setupServer(serverPort, serverAddress);
            RESTManager.getInstance.start();

            RESTManager.getInstance.setupClient(clientPort, clientAddress);
            RESTManager.getInstance.connect("Equipment", "21232F297A57A5A743894A0E4A801FC3");
        }

        private void SmartConnector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ConsoleMessages":
                    ConsoleMessages = ConsoleService.getInstance.ConsoleMessages;
                    break;
                case "CommState":
                    CommunicationState = ConsoleService.getInstance.CommState;
                    break;
                case "ControlState":
                    ControlState = ConsoleService.getInstance.ControlState;
                    break;
                case "RestLogMessages":
                    RestLogMessages = ConsoleService.getInstance.RestLogMessages;
                    break;
            }
        }

        public void EnableCommunicationState()
        {
            ConsoleService.getInstance.WriteToConsole("Communication state request : Enable");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("EnableOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EnableOperatorRequest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EnableOperatorRequest not sent.");
            }
        }

        public void DisableCommunicationState()
        {
            ConsoleService.getInstance.WriteToConsole("Communication state request : Disable");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("DisableOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] DisableOperatorRqeuest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] DisableOperatorRequest not sent.");
            }
        }

        public void OfflineControlState()
        {
            ConsoleService.getInstance.WriteToConsole("Control state request : Offline");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("OfflineOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] OfflineOperatorRequest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] OfflineOperatorRequest not sent.");
            }
        }

        public void OnlineControlState()
        {
            ConsoleService.getInstance.WriteToConsole("Control state request : Online");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("OnlineOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] OnlineOperatorRequest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] OnlineOperatorRequest not sent.");
            }
        }

        public void RemoteControlState()
        {
            ConsoleService.getInstance.WriteToConsole("Control state request : Remote");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("RemoteOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] RemoteOperatorRequest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] RemoteOperatorRequest not sent.");
            }
        }

        public void LocalControlState()
        {
            ConsoleService.getInstance.WriteToConsole("Control state request : Local");

            JObject data = new JObject();

            if (RESTManager.getInstance.sendEvent("LocalOperatorRequest", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] LocalOperatorRequest sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] LocalOperatorRequest not sent.");
            }
        }

        public void SendEvent()
        {
            ConsoleService.getInstance.WriteToConsole("Send event notification");

            JObject data = new JObject();
            JObject predefinedDvList = new JObject();
            data.Add("EventName", EventName);
            data.Add("PredefinedDVList", predefinedDvList);

            if (RESTManager.getInstance.sendEvent("SendEventNotification", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] SendEventNotification sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] SendEventNotification not sent.");
            }
        }

        public void AlarmSet()
        {
            ConsoleService.getInstance.WriteToConsole("Alarm set");

            JObject data = new JObject();
            JObject predefinedDvList = new JObject();
            data.Add("AlarmName", AlarmName);
            data.Add("IsSet", true);
            data.Add("PredefinedDVList", predefinedDvList);

            if (RESTManager.getInstance.sendEvent("EquipmentAlarmOccurrence", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EquipmentAlarmOccurence : Set sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EquipementAlarmOccurence : Set not sent.");
            }
        }

        public void AlarmClear()
        {
            ConsoleService.getInstance.WriteToConsole("Alarm clear");

            JObject data = new JObject();
            JObject predefinedDvList = new JObject();
            data.Add("AlarmName", AlarmName);
            data.Add("IsSet", false);
            data.Add("PredefinedDvList", predefinedDvList);

            if (RESTManager.getInstance.sendEvent("EquipmentAlarmOccurence", data, ""))
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EquipmentAlarmOccurence : Clear sent.");
            } else
            {
                ConsoleService.getInstance.WriteToConsole("[OUT] EquipmentAlarmOccurence : Clear not sent.");
            }
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
