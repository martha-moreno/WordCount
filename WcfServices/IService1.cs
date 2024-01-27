using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        List<string> Map(int N, string[] words);

        [OperationContract]
        List<string> Reduce(string partition);

        [OperationContract]
        //  List<KeyValuePair<string, int>> Combiner(List<KeyValuePair<string, int>> keyValueList);
        List<KeyValuePair<string, int>> Combiner(string [] reducersArray);

    }

}
    