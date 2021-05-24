using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Error;
using Ca.Infoway.Messagebuilder.Marshalling;
using Ca.Infoway.Messagebuilder.Marshalling.HL7;
using Ca.Infoway.Messagebuilder.Model;
using Ca.Infoway.Messagebuilder.Util.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Home.ClinicalPortal
{
    public static class HL7Serializer
    {
        public static TInteraction Deserialize<TInteraction>(RenderMode renderMode, SpecificationVersion version, string xmlString) 
            where TInteraction : IInteraction
        {
            XmlDocument doc = new DocumentFactory().CreateFromString(xmlString);
            XmlToModelResult result =
                new MessageBeanTransformerImpl(renderMode).TransformFromHl7(version, doc);

            var errors = result.GetHl7Errors();

            if (errors.Count > 0)
            {
                // iterating through the errors detected by the MB validation component 
                foreach (Hl7Error hl7Error in result.GetHl7Errors())
                {
                    Console.WriteLine(hl7Error);
                }
            }

            return (TInteraction)result.GetMessageObject();
        }

        public static string Serialize(RenderMode renderMode, SpecificationVersion version, IInteraction response)
        {
            // The transformer object which can convert objects <-> xml.
            // Note that it is set to permissive mode in order to allow errors to occur without throwing an exception; this
            // may be a desired setting even in production. 
            MessageBeanTransformerImpl messageBeanTransformer = new MessageBeanTransformerImpl(renderMode);

            // transform the message object into its xml representation (also performs validation)
            ModelToXmlResult result = messageBeanTransformer.TransformToHl7AndReturnResult(version, response);

            return result.GetXmlMessage();
        }
    }
}
