using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SmartConnector.RESTServices
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet]
        string EchoWithGet(string s);

        /*[OperationContract]
        [WebInvoke(UriTemplate = "/HMI/Event", ResponseFormat = WebMessageFormat.Json)]
        void GetEvent();*/

        [OperationContract]
        [WebInvoke(UriTemplate = "/HMI/Event", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void GetEvent(Stream streamdata);
       

        [OperationContract]
        [WebGet(UriTemplate = "/GetSoftwareVersion", ResponseFormat = WebMessageFormat.Json)]
        String GetSoftwareVersion();
    }
}
