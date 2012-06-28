﻿using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.Xml.Dom;
using Jabber.Net.Server.Handlers;
using Jabber.Net.Server.Sessions;

namespace Jabber.Net.Server.S2C
{
    class RosterHandler : XmppHandler, IXmppHandler<RosterIq>
    {
        [IQType(IqType.get, IqType.set)]
        public XmppHandlerResult ProcessElement(RosterIq element, XmppSession session, XmppHandlerContext context)
        {
            var to = element.HasTo ? element.To : session.Jid;
            if (to != session.Jid)
            {
                return Error(session, ErrorCondition.Forbidden, element);
            }

            if (element.Type == IqType.get)
            {
                foreach (var ri in context.Storages.Users.GetRosterItems(to.User))
                {
                    element.Query.AddRosterItem(ri);
                }
                element.ToResult();
                session.RosterRequest();
                return Send(session, element);
            }
            else
            {
                if (element.Query.GetRoster().Length != 1)
                {
                    return Error(session, ErrorCondition.BadRequest, element);
                }

                var result = Component();
                var ri = element.Query.GetRoster()[0];

                // send all available user's resources
                foreach (var s in context.Sessions.FindSessions(session.Jid.BareJid))
                {
                    result.AddResult(Send(s, (Element)element.Clone()));
                }

                if (ri.Subscription != SubscriptionType.remove)
                {
                    context.Storages.Users.SaveRosterItem(to.User, ri);
                }
                else
                {
                    context.Storages.Users.RemoveRosterItem(to.User, ri.Jid);

                    var unsubscribe = new Presence { Type = PresenceType.unsubscribe, To = ri.Jid, From = session.Jid };
                    var unsubscribed = new Presence { Type = PresenceType.unsubscribed, To = ri.Jid, From = session.Jid };
                    var unavailable = new Presence { Type = PresenceType.unavailable, To = ri.Jid, From = session.Jid };
                    var sended = false;
                    foreach (var s in context.Sessions.FindSessions(ri.Jid.BareJid))
                    {
                        if (s.Rostered)
                        {
                            result.AddResult(Send(s, unsubscribe, true));
                            result.AddResult(Send(s, unsubscribed, true));
                            sended = true;
                        }
                        result.AddResult(Send(s, unavailable));
                    }
                    if (!sended)
                    {
                        context.Storages.Elements.SaveElements(ri.Jid, "offline", unsubscribe, unsubscribed);
                    }
                }
                
                element.ToResult();
                result.AddResult(Send(session, element));
                return result;
                //send all available user's resources

                /*catch (Exception)
                {
                    // restore
                    roster.RemoveAllChildNodes();
                    item = context.StorageManager.RosterStorage.GetRosterItem(iq.From, item.Jid);
                    if (item != null)
                    {
                        roster.AddRosterItem(item.ToRosterItem());
                        context.Sender.Broadcast(context.SessionManager.GetBareJidSessions(iq.From), iq);
                    }
                    throw;
                }*/
            }
        }
    }
}
