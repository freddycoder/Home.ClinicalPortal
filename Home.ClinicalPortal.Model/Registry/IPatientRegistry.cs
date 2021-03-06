using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Interaction;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Home.ClinicalPortal.Model.Registry
{
    [ServiceContract]
    public interface IPatientRegistry
    {
        [OperationContract]
        string AddPatient(string request);

        [OperationContract]
        string GetDemographics(string request);

        [OperationContract]
        Task<string> FindCandidates(string request);

        [OperationContract]
        string RecordRevised(string request);
    }
}
