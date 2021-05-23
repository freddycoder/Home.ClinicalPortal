using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Marshalling;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Cr.Prpa_mt101103ca;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Mfmi_mt700751ca;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Interaction;
using Ca.Infoway.Messagebuilder.Resolver.Configurator;
using System;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Merged;
using Ca.Infoway.Messagebuilder.Datatype.Lang;
using Ca.Infoway.Messagebuilder.Domainvalue.Payload;
using Ca.Infoway.Messagebuilder.Resolver;
using Ca.Infoway.Messagebuilder.Marshalling.HL7;
using Ca.Infoway.Messagebuilder.Error;
using Ca.Infoway.Messagebuilder.Codesystem;
using System.ServiceModel;
using FindCandidate.Model;
using Ca.Infoway.Messagebuilder.Domainvalue.Transport;

namespace NetCoreConsoleApp
{
    class Program
    {
        static void Main()
        {
            HomeClinicalPortal();
        }

        static void HomeClinicalPortal()
        {
            int choice;
            do
            {
                Console.WriteLine("##########################################");
                Console.WriteLine("#            Home.ClinicalProtal         #");
                Console.WriteLine("##########################################");


                Console.WriteLine("#                                        #");
                Console.WriteLine("# 1. AddPerson                           #");
                Console.WriteLine("# 2. FindCandidate                       #");
                Console.WriteLine("#                                        #");
                Console.Write("# Your choice : ");

                try
                {
                    choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 0:
                            break;
                        case 1:
                            throw new NotImplementedException();
                        case 2:
                            FindCandidateScenario();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception e)
                {
                    do
                    {
                        Console.Error.WriteLine(e.Message);
                        Console.Error.WriteLine(e.StackTrace);
                        Console.Error.WriteLine();

                        e = e.InnerException;
                    } while (e != null);

                    choice = -1;
                }

            } while (choice != 0);
        }

        private static void FindCandidateScenario()
        {
            // the HL7v3 version to target
            SpecificationVersion version = SpecificationVersion.R02_04_03;

            // set up MB default code handling (not likely sufficient for a production environment!)
            // - see the Code Resolvers in User Guide for more details
            DefaultCodeResolutionConfigurator.ConfigureCodeResolversWithTrivialDefault();

            // create the interaction
            var findCandidateQuery = HL7Builder.Build<FindCandidatesQuery>();

            //findCandidateQuery.ProfileId.Add(new Identifier(, ,version.Name))
            findCandidateQuery.ProcessingModeCode = ProcessingMode.CURRENT_PROCESSING;

            findCandidateQuery.ProfileId.Add(new Identifier("1.1.1.1", "R02_04_03"));

            //findCandidateQuery.AcceptAckCode.
            findCandidateQuery.Receiver = new Receiver
            {
                DeviceName = "FindCandiate",
                DeviceId = new Identifier("2.16.578.1.34.1", "221")
            };

            findCandidateQuery.Sender = new Sender
            {
                DeviceName = "FindCandidate.Client",
                DeviceSoftwareName = "FindCandidate.Client ConsoleApp",
                DeviceDesc = "Client to test and demo the solution",
                DeviceId = new Identifier("2.16.578.1.34.1", "222")
            };

            findCandidateQuery.AcceptAckCode = AcknowledgementCondition.ALWAYS;

            // create the controlAct and set it on the interaction 
            TriggerEvent<ParameterList> controlAct = new();
            controlAct.Id = new Identifier(Guid.NewGuid().ToString());
            findCandidateQuery.ControlActEvent = controlAct;

            controlAct.Author = new CreatedBy_2
            {
                Time = new PlatformDate(DateTime.Now)
            };

            controlAct.Code = HL7TriggerEventCode.FIND_CANDIDATES_QUERY;

            // create an "intermediary" bean and set it on the controlAct 
            QueryByParameter<ParameterList> queryByParameter = new();
            queryByParameter.QueryId = new Identifier(Guid.NewGuid().ToString());
            controlAct.QueryByParameter = queryByParameter;

            // create the payload bean and set it on the "intermediary" bean 
            ParameterList parameterList = new();
            queryByParameter.ParameterList = parameterList;

            // setting values on the payload
            PersonName name = PersonName.CreateFirstNameLastName("Frédéric", "Jacques");
            parameterList.PersonNameValue.Add(name);
            parameterList.AdministrativeGenderValue = AdministrativeGender.MALE;

            // alternative way to create a code object via direct lookup
            Ca.Infoway.Messagebuilder.Domainvalue.AdministrativeGender codeLookup =
                CodeResolverRegistry.Lookup<AdministrativeGender>(
                "M", CodeSystem.VOCABULARY_ADMINISTRATIVE_GENDER.Root);

            parameterList.AdministrativeGenderValue = codeLookup;

            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(new Uri("http://localhost:5050/Registry/PatientRegistry.svc"));
            var channelFactory = new ChannelFactory<IPatientRegistry>(binding, endpoint);
            var serviceClient = channelFactory.CreateChannel();
            var reponseString = serviceClient.FindCandidates(HL7Serializer.Serialize(RenderMode.PERMISSIVE, version, findCandidateQuery));
            Console.WriteLine(reponseString);
            var findCandidateResponse = HL7Serializer.Deserialize<FindCandidatesResponse>(RenderMode.PERMISSIVE, version, reponseString);
            Console.WriteLine(findCandidateResponse.ToString());
        }
    }
}
