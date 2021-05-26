using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Datatype.Lang;
using Ca.Infoway.Messagebuilder.Error;
using Ca.Infoway.Messagebuilder.J5goodies;
using Ca.Infoway.Messagebuilder.Marshalling;
using Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote;
using Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Merged;
using Ca.Infoway.Messagebuilder.Resolver.Configurator;
using FHIRProxy;
using Home.ClinicalPortal.Model.Laboratory;
using Platform.Xml.Sax;
using System;

namespace Laboratory.Contract
{
	public class LaboratoryContract : ILaboratory
	{
        private readonly FHIRClient context;

        public LaboratoryContract(FHIRClient context)
        {
            this.context = context;
        }

        public string QueryResults(string request)
        {
            // the HL7v3 version to target
            SpecificationVersion version = SpecificationVersion.CCDA_R1_1;

            // set up MB default code handling (not likely sufficient for a production environment!)
            // - see the Code Resolvers in User Guide for more details
            DefaultCodeResolutionConfigurator.ConfigureCodeResolversWithTrivialDefault();

            // create the document
            ConsultationNoteDocument consultationNote = new ConsultationNoteDocument();

            // create basic values for some fields
            consultationNote.TemplateId.Add(new Identifier("2.16.840.1.113883.10.20.22.1.4"));
            consultationNote.Title = "Community Health and Hospitals: Consultation Note";
            consultationNote.EffectiveTime = new MbDate(DateUtil.GetDate(2012, 8, 16));

            consultationNote.Component = CreateComponent();

            // The transformer object which can convert documents <-> xml.
            // Note that it is set to permissive mode in order to allow errors to occur without throwing an exception; this
            // may be a desired setting even in production.
            ClinicalDocumentTransformer documentTransformer = new ClinicalDocumentTransformer();

            // transform the message object into its xml representation (also performs validation) 
            CdaModelToXmlResult result = documentTransformer.TransformToDocument(
                version, consultationNote);

            // the message produced (notice the errors listed as comments within the xml message) 
            var xml = result.GetXmlDocument();
            Console.WriteLine(xml);

            // iterating through the errors detected by the MB validation component 
            foreach (TransformError error in result.GetErrors())
            {
                Console.WriteLine(error);
            }

            return xml;
        }

        private Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote.Component2 CreateComponent()
        {
            Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote.StructuredBody body =
                new Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote.StructuredBody();
            body.Component.Add(CreateReasonForVisit());

            body.Component.Add(CreateGeneralStatus());

            Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote.Component2 component =
                new Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Consultationnote.Component2
                {
                    Component2Choice = body
                };
            return component;
        }

        private GeneralStatusSectionComponent3 CreateGeneralStatus()
        {
            EncapsulatedData text = new EncapsulatedData();
            try
            {

                text.AddContent("<paragraph>Alert and in good spirits, no acute distress.</paragraph>");
            }
            catch (SAXException e)
            {
                // nothing to do here
            }

            Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Generalstatussection.Section section =
                new Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Generalstatussection.Section
                {
                    Title = "GENERAL STATUS",
                    Text = text
                };

            GeneralStatusSectionComponent3 generalStatus = new GeneralStatusSectionComponent3
            {
                Section = section
            };

            return generalStatus;
        }

        private ChiefComplaintAndReasonForVisitSectionComponent3 CreateReasonForVisit()
        {
            EncapsulatedData text = new EncapsulatedData();

            try
            {
                text.AddContent("<paragraph>Dark stools.</paragraph>");
            }
            catch (SAXException e)
            {
                // nothing to do here
            }

            Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Chiefcomplaintandreasonforvisitsection.Section section =
                new Ca.Infoway.Messagebuilder.Model.Ccda_r1_1.Chiefcomplaintandreasonforvisitsection.Section
                {
                    Title = "REASON FOR VISIT/CHIEF COMPLAINT",

                    Text = text
                };

            ChiefComplaintAndReasonForVisitSectionComponent3 reasonForVisit =
                new ChiefComplaintAndReasonForVisitSectionComponent3
                {
                    Section = section
                };

            return reasonForVisit;
        }
    }
}
