using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Marshalling;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Interaction;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101103ca;
using Ca.Infoway.Messagebuilder.Resolver.Configurator;
using System;
using System.Linq;
using Ca.Infoway.Messagebuilder.Datatype.Lang;
using static Home.ClinicalPortal.HL7Serializer;
using Microsoft.EntityFrameworkCore;
using Ca.Infoway.Messagebuilder.Domainvalue.Payload;
using Ca.Infoway.Messagebuilder.Domainvalue.Transport;
using Ca.Infoway.Messagebuilder.Resolver;
using Ca.Infoway.Messagebuilder.Codesystem;
using Home.ClinicalPortal;
using Home.ClinicalPortal.Model.Registry;
using FHIRProxy;
using System.Threading.Tasks;
using Hl7.Fhir.Serialization;
using System.Text.Encodings.Web;

namespace Registry.Contract
{
	public class PatientRegistryContract : IPatientRegistry
	{
        private readonly FHIRClient context;

        public PatientRegistryContract(FHIRClient context)
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

        public async Task<string>FindCandidates(string request)
        {
            // the HL7v3 version to target
            SpecificationVersion version = SpecificationVersion.R02_04_03;

            // set up MB default code handling (not likely sufficient for a production environment!) - see the Code Resolvers in User Guide for more details
            DefaultCodeResolutionConfigurator.ConfigureCodeResolversWithTrivialDefault();

            var findCandidatesRequest = Deserialize<FindCandidatesQuery>(RenderMode.PERMISSIVE, version, request);
            ParameterList parameterList = findCandidatesRequest.ControlActEvent.QueryByParameter.ParameterList;
            var query = $"";
            foreach (PersonName personName in parameterList.PersonNameValue)
            {
                Console.WriteLine("Name: " + personName.GivenName + " " + personName.FamilyName);
                query += $"?family={personName.FamilyName}&given={personName.GivenName}";
                if (parameterList?.AdministrativeGenderValue?.CodeValue != null)
                {
                    query += $"&gender={ToFhirGender(parameterList.AdministrativeGenderValue.CodeValue)}";
                }
                if (parameterList.PersonBirthtimeValue != null)
                {
                    query += $"&birthdate={((DateTime)parameterList.PersonBirthtimeValue).ToString("yyyy-MM-dd")}";
                }
            }

            var dbResponse = await context.LoadResource("Patient", query);

            Console.WriteLine(dbResponse);

            FhirJsonParser fjp = new FhirJsonParser();
            Hl7.Fhir.Model.Bundle bundle = fjp.Parse<Hl7.Fhir.Model.Bundle>(dbResponse.Content.ToString());

            Hl7.Fhir.Model.Patient fhirPatient = default;
            if (bundle.Entry != null && bundle.Entry.Count >= 2)
            {
               fhirPatient = fjp.Parse<Hl7.Fhir.Model.Patient>(bundle.Entry[1].Children.ElementAt(1).ToJson());
            }

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
                    ResultTotalQuantity = fhirPatient != null ? 1 : 0,
                    ResultCurrentQuantity = fhirPatient != null ? 1 : 0,
                    QueryResponseCode = fhirPatient != null ? QueryResponse.DATA_FOUND : QueryResponse.NO_DATA_FOUND,
                    ResultRemainingQuantity = 0
                }
            };

            var registredRole = new Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101104ca.IdentifiedPerson
            {

            };

            if (fhirPatient != null)
            {
                PersonName name = PersonName.CreateFirstNameLastName(fhirPatient.Name[0].Given.First(), fhirPatient.Name[0].Family);

                registredRole.IdentifiedPersonName.Add(name);

                // alternative way to create a code object via direct lookup
                Ca.Infoway.Messagebuilder.Domainvalue.AdministrativeGender codeLookup =
                    CodeResolverRegistry.Lookup<AdministrativeGender>(
                    fhirPatient.Gender.Value.ToString(), CodeSystem.VOCABULARY_ADMINISTRATIVE_GENDER.Root);
                
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

        private object ToFhirGender(string codeValue)
        {
            if (codeValue == "M")
            {
                return Hl7.Fhir.Model.AdministrativeGender.Male.ToString().ToLower();
            }
            if (codeValue == "F")
            {
                return Hl7.Fhir.Model.AdministrativeGender.Female.ToString().ToLower();
            }

            return Hl7.Fhir.Model.AdministrativeGender.Unknown.ToString().ToLower();
        }

        public string RecordRevised(string request)
        {
            throw new NotImplementedException();
        }
    }
}
