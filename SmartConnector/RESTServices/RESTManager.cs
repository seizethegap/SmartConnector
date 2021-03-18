using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

using SmartConnector.Services;

namespace SmartConnector.RESTServices
{
    public sealed class RESTManager
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static RESTManager instance;
        private static readonly object padlock = new object();

        private int m_sendID = 0;

        private WebServiceHost restHost;

        public RESTManager() { }

        public static RESTManager getInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RESTManager();
                    }
                    return instance;
                }
            }
        }

        public void setupClient(string port, string baseUri)
        {
            logger.Debug("Setting up RESTClient : Base Uri - {0}{1} ", baseUri, port);
            ConsoleService.getInstance.WriteToRestLog("Setting up RESTClient : Base Uri - " + baseUri + " " + port + ".");
            RESTClient.getInstance.BaseUri = baseUri;
            RESTClient.getInstance.Port = port;
        }

        public void connect(string username, string password)
        {

            logger.Debug("Connecting RESTClient : User - {0} , Pwd - {1}", username, password);
            //ConsoleService.getInstance.WriteToRestLog("Connecting RESTClient : User - " + username + " , Pwd - " + password);
            ConsoleService.getInstance.WriteToRestLog("Connecting RESTClient : User - " + username + " , Pwd - ******************");
            RESTClient.getInstance.Username = username;
            RESTClient.getInstance.Password = password;

            RESTClient.getInstance.connect(1);
            RESTClient.getInstance.connect(2);
            RESTClient.getInstance.connect(3);

            sendEvent("OperatorCommunicationStateRequest");

            sendEvent("OperatorControlStateRequest");

        }

        public void setupServer(string port, string baseUri)
        {
            logger.Debug("Setting up RESTServer : Base Uri - {0}{1} ", baseUri, port);
            ConsoleService.getInstance.WriteToRestLog("Setting up RESTServer : Base Uri - " + baseUri + " " + port + ".");
            RESTServer.getInstance.BaseUri = baseUri;
            RESTServer.getInstance.Port = port;
        }

        public void stop()
        {
            restHost.Abort();
        }

        public void disconnect()
        {
            RESTClient.getInstance.disconnect();
        }

        public void start()
        {
            Uri baseAddress = new Uri(RESTServer.getInstance.BaseUri + RESTServer.getInstance.Port);

            restHost = new WebServiceHost(typeof(RESTServer), baseAddress);
            ServiceEndpoint ep = restHost.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "");

            ServiceDebugBehavior sdb = restHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            sdb.HttpHelpPageEnabled = false;

            restHost.Open();

            logger.Debug("Started REST Server : {0}", baseAddress.AbsoluteUri);
            ConsoleService.getInstance.WriteToRestLog("Started REST Server : " + baseAddress.AbsoluteUri);

        }


        public bool sendEvent(string eventName, JObject eventData = null, string eventSendId = "")
        {
            RESTClient.getInstance.EventName = eventName;
            RESTClient.getInstance.EventType = 3; //default value : 3
            RESTClient.getInstance.EventOrigin = RESTClient.getInstance.Username;
            RESTClient.getInstance.EventOriginType = ""; //default value : null
            RESTClient.getInstance.EventInvokeID = ""; //default value : null
            RESTClient.getInstance.EventData = eventData;

            if (eventSendId == "")
            {
                eventSendId = generateId().ToString();
            }

            RESTClient.getInstance.EventSendID = eventSendId;


            return RESTClient.getInstance.sendEvent();

        }

        private int generateId()
        {
            return (m_sendID++);
        }
    }

}

