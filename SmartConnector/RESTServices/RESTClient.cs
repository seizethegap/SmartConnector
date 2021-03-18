using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using RestSharp.Serialization.Json;

using SmartConnector.Services;

public class jLoginObject
{
    public string name { get; set; }
    public string password { get; set; }

    public jLoginObject() { }
}

public class Data
{
    public string Mood { get; set; }
}

public sealed class RESTClient
{
    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    private static RESTClient instance;
    private static readonly object padlock = new object();

    private JObject jsonMessage = new JObject();

    private RestClient m_client;

    public RESTClient() { }

    public void disconnect()
    {
        m_client = null;
    }

    public static RESTClient getInstance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    logger.Debug("Creating RESTClient Instance");
                    ConsoleService.getInstance.WriteToRestLog("Creating RESTClient Instance");
                    instance = new RESTClient();
                }
                return instance;
            }
        }
    }

    public void connect(int step)
    {
        string customURL = string.Format(BaseUri + Port);
        logger.Debug("Client connection [Step {2}] : {0}{1}", BaseUri, Port, step);
        ConsoleService.getInstance.WriteToRestLog("Client connection [Step " + step + "] : " + BaseUri + " " + Port + ".");

        RestRequest req;
        IRestResponse response;
        string debugVar;
        Dictionary<string, string> output;
        var client = new RestClient(BaseUri + Port);
        m_client = client;

        var jObjectbody = new JObject();
        var deserializer = new JsonDeserializer();
      
        switch (step)
        {
            case 1:
                req = new RestRequest("/SmartCore/rest/login", Method.POST);

                
                jObjectbody.Add("name", Username);
                jObjectbody.Add("password", Password);

                req.AddParameter("application/json", jObjectbody, ParameterType.RequestBody);

                logger.Debug("Request -- Body : {0}", jObjectbody.ToString());
                ConsoleService.getInstance.WriteToRestLog("Request -- Body : " + jObjectbody.ToString());
                response = client.Execute(req);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.Error("Incorrect Response after request");
                    ConsoleService.getInstance.WriteToRestLog("Incorrect Response after request");
                }

                logger.Debug("Response -- Body : {0}", response.Content);
                ConsoleService.getInstance.WriteToRestLog("Response -- Body : " + response.Content);

                output = deserializer.Deserialize<Dictionary<string, string>>(response);

                //debug
                debugVar = output["name"];
                Debug.Assert(debugVar == Username);

                AgentId = output["id"];

                AuthorizationToken = response.Headers.ToList()
                .Find(x => x.Name == "X-Authorization")
                .Value.ToString();

                logger.Debug("ID : {0} - AuthorizationKey : {1}", AgentId, AuthorizationToken);
                ConsoleService.getInstance.WriteToRestLog("ID : " + AgentId + " - AuthorizationKey : " + AuthorizationToken);
                break;

            case 2:
                var strBuilder = "/SmartCore/rest/agent/" + AgentId;
                req = new RestRequest(strBuilder, Method.GET);

                response = client.Execute(req);
                logger.Debug("Response -- Body : {0}", response.Content);
                ConsoleService.getInstance.WriteToRestLog("Response -- Body : " + response.Content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.Error("Incorrect Response after GET request");
                    ConsoleService.getInstance.WriteToRestLog("Incorrect Response after GET Request");
                }

                output = deserializer.Deserialize<Dictionary<string, string>>(response);

                jObjectbody.Add("callbackPort", "8088");
                jObjectbody.Add("callbackPath", "Event");
                jObjectbody.Add("callbackRoot", "HMI");
                jObjectbody.Add("callbackHostname", "127.0.0.1");
                jObjectbody.Add("displayName", "GUI");
                jObjectbody.Add("password", Password);
                jObjectbody.Add("email", "Support@focussia.com");

                req = new RestRequest(strBuilder, Method.POST);
                req.AddParameter("application/json", jObjectbody, ParameterType.RequestBody);
                req.AddParameter("Authorization", AuthorizationToken, ParameterType.HttpHeader);
                //req.AddHeader("Authorization", AuthorizationToken);

                logger.Debug("Request -- Body : {0}", jObjectbody.ToString());
                ConsoleService.getInstance.WriteToRestLog("Request -- Body : " + jObjectbody.ToString());
                response = client.Execute(req);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.Error("Incorrect Response after POST request");
                    ConsoleService.getInstance.WriteToRestLog("Incorrect Response after POST Request");
                    System.Environment.Exit(1);
                }

                break;

            case 3:

                logger.Debug("Client initialization completed");
                ConsoleService.getInstance.WriteToRestLog("Client initialization completed.");
                break;

            default:
                logger.Error("State unknown");
                ConsoleService.getInstance.WriteToRestLog("State unknown");
                break;
        }

        //update client variable
        m_client = client;

    }

    public bool sendEvent()
    {
        bool res = true;

        jsonMessage.Add("name", EventName);
        jsonMessage.Add("type",EventType );
        jsonMessage.Add("origin", EventOrigin);
        jsonMessage.Add("originType", EventOriginType);
        jsonMessage.Add("invokeid", EventInvokeID);
        jsonMessage.Add("sendid", EventSendID);
        jsonMessage.Add("data", EventData);

        logger.Debug("Sending event ... \n JSON message : {0} ", jsonMessage.ToString());
        ConsoleService.getInstance.WriteToRestLog("Sending event ... ");
        ConsoleService.getInstance.WriteToRestLog("JSON message : " + jsonMessage.ToString());

        var req = new RestRequest("/SmartCore/rest/event/GemAS", Method.POST);

        req.AddParameter("application/json", jsonMessage, ParameterType.RequestBody);
        req.AddParameter("Authorization", AuthorizationToken, ParameterType.HttpHeader);

        var response = m_client.Execute(req);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            logger.Error("Incorrect Response after request");
            ConsoleService.getInstance.WriteToRestLog("Incorrect Response after request");
            res = false;
        }

        //reset message variable : TODO change to local
        jsonMessage = new JObject();

        return res;

    }

    //Properties

    public string Port { get; set; }

    public string BaseUri { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string AgentId { get; set; }

    public string AuthorizationToken { get; set; }

    /* Event name property. */
    public string EventName { get; set; }

    /* Event type property. */
    public int EventType { get; set; } //not used , value is always 3

    /* Event origin property */
    public string EventOrigin { get; set; }

    /* Event origin type property*/
    public string EventOriginType { get; set; } //not used , always null

    /* Event invoke ID property */
    public string EventInvokeID { get; set; } // not used , always null

    /* Event send ID property */
    public string EventSendID { get; set; } // not used ,  always null

    /* Event data property */
    public JObject EventData { get; set; }

    /* Event target ID property */
    private string EventTarget { get; set; }

}


