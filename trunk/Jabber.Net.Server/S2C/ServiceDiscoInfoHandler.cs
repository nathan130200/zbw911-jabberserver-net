﻿using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.disco;
using Jabber.Net.Server.Handlers;
using Jabber.Net.Server.Sessions;

namespace Jabber.Net.Server.S2C
{
    class ServiceDiscoInfoHandler : XmppHandler, IXmppHandler<DiscoInfoIq>
    {
        private readonly DiscoInfoIq iq;

        
        public ServiceDiscoInfoHandler(ServiceInfo serviceInfo, string[] features)
        {
            Args.NotNull(serviceInfo, "serviceInfo");
            Args.NotNull(features, "features");

            iq = new DiscoInfoIq(IqType.result);

            iq.Query.AddIdentity(serviceInfo.Category, serviceInfo.Type, serviceInfo.Name);
            foreach (var feature in features)
            {
                iq.Query.AddFeature(feature);
            }
        }

        [IQType(ErrorCondition.BadRequest, IqType.get)]
        public XmppHandlerResult ProcessElement(DiscoInfoIq element, XmppSession session, XmppHandlerContext context)
        {
            var result = (DiscoInfoIq)iq.Clone();
            result.Id = element.Id;
            result.To = session.Jid;
            result.From = element.To;
            return Send(session, result);
        }
    }
}
