using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using System.IO;
using RestSharp.Serialization.Json;

using SmartConnector.Services;
using System.ComponentModel;

namespace SmartConnector.RESTServices
{
    public sealed class RESTServer : IService
    {
        public RESTServer() { }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static RESTServer instance;
        private static readonly object padlock = new object();


        public static RESTServer getInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        logger.Debug("Creating RESTServer Instance");
                        ConsoleService.getInstance.WriteToRestLog("Creating RESTServer Instance");
                        instance = new RESTServer();
                    }
                    return instance;
                }
            }
        }

        public String GetSoftwareVersion()
        {
            return "v1.00";
        }
        public string EchoWithGet(string s)
        {
            return "You said " + s;
        }

        public void GetEvent(Stream streamdata)
        {
            logger.Debug("Event received");
            ConsoleService.getInstance.WriteToRestLog("Event received");

            StreamReader reader = new StreamReader(streamdata);
            string res = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            logger.Debug("Event : {0}", res);
            ConsoleService.getInstance.WriteToRestLog("Event : " + res);

            var json = JObject.Parse(res);

            onEventReceived(json);
        }

        private void onEventReceived(JObject json)
        {

            string m_event = json["name"].ToString();

            switch (m_event)
            {
                case "CommunicationStateChanged":
                    onCommunicationStateChanged(json);
                    break;

                //using communication state changed method as the same data is returned from SMARTGEM
                case "OperatorCommunicationStateReply":
                    onCommunicationStateChanged(json);
                    break;

                case "ControlStateChanged":
                    onControlStateChanged(json);
                    break;

                //using control state changed method as the same data is returned from SMARTGEM
                case "OperatorControlStateReply":
                    onControlStateChanged(json);
                    break;

                case "AlarmStateChanged":
                    onAlarmStateChanged(json);
                    break;

                case "EnhancedRemoteCommandRequest":
                    onEnhancedRemoteCommandRequestChanged(json);
                    break;

                case "RemoteCommandRequest":
                    onRemoteCommandRequestChanged(json);
                    break;

                default:
                    //logger.Warn("Event not treated");
                    ConsoleService.getInstance.WriteToConsole("Event not treated");
                    break;
            }

        }

        private void onCommunicationStateChanged(JObject json)
        {
            ConsoleService.getInstance.CommState = json["data"]["CurrentCommunicationState"].ToString();
        }

        private void onControlStateChanged(JObject json)
        {
            ConsoleService.getInstance.ControlState = json["data"]["CurrentControlState"].ToString();
        }

        private void onAlarmStateChanged(JObject json)
        {
            string name = json["data"]["AlarmName"].ToString();
            string state = json["data"]["IsSet"].ToString();
            ConsoleService.getInstance.WriteToConsole("[IN] Alarm state changed : " + name + " - " + state + ".");
        }

        private void onEnhancedRemoteCommandRequestChanged(JObject json)
        {
            ConsoleService.getInstance.WriteToConsole("EnhancedRemoteCommandRequest received.");
            List<string> LotsList = new List<string>();
            List<string> PpsList = new List<string>();

            int LotsListSize = json["data"]["PARAMETERS"]["LOTID_LIST"].Count();
            int PpsListSize = json["data"]["PARAMETERS"]["PPID_LIST"].Count();
            ConsoleService.getInstance.WriteToConsole(LotsListSize.ToString() + " IDs detected for LOTID_LIST");
            ConsoleService.getInstance.WriteToConsole(PpsListSize.ToString() + " IDs detected for PPID_LIST");

            for (int i = 0; i < LotsListSize; i++)
            {
                LotsList.Add(json["data"]["PARAMETERS"]["LOTID_LIST"][i]["value"][0].ToString());
                PpsList.Add(json["data"]["PARAMETERS"]["PPID_LIST"][i]["value"][0].ToString());
            }

            StringBuilder sbLot = new StringBuilder();
            foreach (var lotid in LotsList)
            {
                sbLot.Append(lotid + ", ");
            }

            StringBuilder sbPp = new StringBuilder();
            foreach (var ppid in PpsList)
            {
                sbPp.Append(ppid + ", ");
            }

            ConsoleService.getInstance.WriteToConsole("LOTID_LIST: " + sbLot.ToString());
            ConsoleService.getInstance.WriteToConsole("PPID_LIST: " + sbPp.ToString());

            var state = json["data"]["OBJSPEC"].ToString();
            var rcmd = json["data"]["RCMD"].ToString();
            var sendid = json["sendid"].ToString();


        }

        private void onRemoteCommandRequestChanged(JObject json)
        {
            var rcmd = json["data"]["OBJSPEC"].ToString();
            var sendid = json["sendid"].ToString();
        }

        public string Port { get; set; }

        public string BaseUri { get; set; }
    }
}
