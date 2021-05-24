using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Interaction;
using System;
using System.ServiceModel;

namespace Home.ClinicalPortal.Model.Registry
{
    [ServiceContract]
    public interface IPersonRegistry
    {
        [OperationContract]
        string AddPatient(string request);

        [OperationContract]
        string GetDemographics(string request);

        [OperationContract]
        string FindCandidates(string request);

        [OperationContract]
        string RecordRevised(string request);
    }
}
