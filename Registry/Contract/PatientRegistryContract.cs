using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Marshalling;
using Ca.Infoway.Messagebuilder.Marshalling.HL7;
using Ca.Infoway.Messagebuilder.Model;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Interaction;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101103ca;
using Ca.Infoway.Messagebuilder.Resolver.Configurator;
using Ca.Infoway.Messagebuilder.Util.Xml;
using FindCandidate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using Ca.Infoway.Messagebuilder.Datatype.Lang;
using Ca.Infoway.Messagebuilder.Error;
using static FindCandidate.Model.HL7Serializer;
using FindCandidate.Data;
using Ca.Infoway.Messagebuilder.Datatype.Lang.Util;
using Microsoft.EntityFrameworkCore;
using Ca.Infoway.Messagebuilder.Domainvalue.Payload;
using Ca.Infoway.Messagebuilder.Domainvalue.Transport;
using Ca.Infoway.Messagebuilder.Resolver;
using Ca.Infoway.Messagebuilder.Codesystem;

namespace MyHL7.Contract
{
	public class PatientRegistryContract : IPatientRegistry
	{
        private readonly FindCandidateDbContext context;

        public PatientRegistryContract(FindCandidateDbContext context)
        {
            this.context = context;
        }

        public string AddPatient(string request)
        {
            throw new NotImplementedException();
        }

        public string GetDemographics(string request)
        {
            throw new NotImplementedException();
        }

        public string FindCandidates(string request)
        {
            // the HL7v3 version to target
            SpecificationVersion version = SpecificationVersion.R02_04_03;

            // set up MB default code handling (not likely sufficient for a production environment!) - see the Code Resolvers in User Guide for more details
            DefaultCodeResolutionConfigurator.ConfigureCodeResolversWithTrivialDefault();

            var findCandidatesRequest = Deserialize<FindCandidatesQuery>(RenderMode.PERMISSIVE, version, request);
            ParameterList parameterList = findCandidatesRequest.ControlActEvent.QueryByParameter.ParameterList;
            foreach (PersonName personName in parameterList.PersonNameValue)
            {
                Console.WriteLine("Name: " + personName.GivenName + " " + personName.FamilyName);
            }

            var dbResponse = context.Patients.AsNoTracking().FirstOrDefault();

            var response = HL7Builder.Build<FindCandidatesResponse>();

            response.ProfileId.Add(new Identifier("1.1.1.1", "R02_04_03"));

            response.Acknowledgement = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.Acknowledgement
            {
                TargetMessageId = new Identifier(Guid.NewGuid().ToString()),

                TypeCode = AcknowledgementType.APPLICATION_ACKNOWLEDGEMENT_ACCEPT
            };

            response.Receiver = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.Receiver
            {
                DeviceName = "FindCandidate.Client",
                DeviceId = new Identifier("2.16.578.1.34.1", "222")
            };

            response.Sender = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.Sender
            {
                DeviceSoftwareName = "FindCandidate SoapCore",
                DeviceName = "FindCandidate",
                DeviceId = new Identifier("2.16.578.1.34.1", "221")
            };

            response.ControlActEvent = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Mfmi_mt700746ca.TriggerEvent<ParameterList, Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101104ca.IdentifiedPerson>
            {
                QueryAck = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.QueryAck
                {
                    QueryId = new Identifier(Guid.NewGuid().ToString()),
                    ResultTotalQuantity = context.Patients.Count(),
                    ResultCurrentQuantity = dbResponse != null ? 1 : 0,
                    QueryResponseCode = dbResponse != null ? QueryResponse.DATA_FOUND : QueryResponse.NO_DATA_FOUND,
                    ResultRemainingQuantity = dbResponse != null ? context.Patients.Count() - 1 : 0
                }
            };

            var registredRole = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101104ca.IdentifiedPerson
            {

            };

            if (dbResponse != null)
            {
                PersonName name = PersonName.CreateFirstNameLastName(dbResponse.FirstName, dbResponse.LastName);

                registredRole.IdentifiedPersonName.Add(name);

                // alternative way to create a code object via direct lookup
                Ca.Infoway.Messagebuilder.Domainvalue.AdministrativeGender codeLookup =
                    CodeResolverRegistry.Lookup<AdministrativeGender>(
                    dbResponse.Gender, CodeSystem.VOCABULARY_ADMINISTRATIVE_GENDER.Root);

                parameterList.AdministrativeGenderValue = codeLookup;
            }

            response.ControlActEvent.SubjectRegistrationEvent.Add(new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.RegistrationEvent<Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101104ca.IdentifiedPerson>
            {
                Subject = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.RegisteredItem<Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101104ca.IdentifiedPerson>
                {
                    RegisteredRole = registredRole
                }
            });

            response.ControlActEvent.QueryByParameter = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged.QueryByParameter<ParameterList>
            {
                QueryId = findCandidatesRequest.ControlActEvent.QueryByParameter.QueryId,
                ParameterList = findCandidatesRequest.ControlActEvent.QueryByParameter.ParameterList
            };

            response.ControlActEvent.Id = new Identifier(Guid.NewGuid().ToString());

            return Serialize(RenderMode.PERMISSIVE, version, response);
        }

        public string RecordRevised(string request)
        {
            throw new NotImplementedException();
        }
    }
}
