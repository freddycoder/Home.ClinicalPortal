using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Home.ClinicalPortal.Model.Laboratory
{
    [ServiceContract]
    public interface ILaboratory
    {
        [OperationContract]
        string QueryResults(string request);
    }
}
