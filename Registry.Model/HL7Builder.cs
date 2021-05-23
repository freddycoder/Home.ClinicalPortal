using Ca.Infoway.Messagebuilder;
using Ca.Infoway.Messagebuilder.Datatype.Lang;
using Ca.Infoway.Messagebuilder.Domainvalue.Transport;
using Ca.Infoway.Messagebuilder.Model;
using Ca.Infoway.Messagebuilder.Model.Pcs_mr2009_r02_04_03.Common.Mcci_mt002100ca;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindCandidate.Model
{
    public static class HL7Builder
    {
        public static TInteraction Build<TInteraction>() where TInteraction : IInteraction
        {
            var interaction = Activator.CreateInstance<TInteraction>();

            var t = typeof(TInteraction);

            t.GetProperty("Id").SetValue(interaction, new Identifier(Guid.NewGuid().ToString()));
            t.GetProperty("CreationTime").SetValue(interaction, new PlatformDate(DateTime.Now));
            t.GetProperty("ResponseModeCode").SetValue(interaction, ResponseMode.IMMEDIATE);
            t.GetProperty("ProcessingCode").SetValue(interaction, ProcessingID.DEBUGGING);
            t.GetProperty("AcceptAckCode").SetValue(interaction, AcknowledgementCondition.ALWAYS);
            

            return interaction;
        }
    }
}
