﻿using System;
using agsXMPP.Xml.Dom;
using Jabber.Net.Server.Handlers;
using Jabber.Net.Server.Xmpp;

namespace Jabber.Net.Server.Connections
{
    class XmppConnection : IXmppReciever, IXmppEndPoint
    {
        private readonly IXmppConnection connection;
        private readonly XmppHandlerManager handlerManager;
        private readonly XmppStreamParser parser;


        public string SessionId
        {
            get;
            set;
        }


        public XmppConnection(IXmppConnection connection, XmppHandlerManager handlerManager)
        {
            Args.NotNull(connection, "connection");
            Args.NotNull(handlerManager, "handlerManager");

            this.connection = connection;
            this.handlerManager = handlerManager;
            this.parser = new XmppStreamParser();

            parser.Parsed += (s, e) => handlerManager.ProcessElement(this, e.Element);
            parser.Error += (s, e) => handlerManager.ProcessError(this, e.Error);
        }

        public void BeginReceive()
        {
            connection.BeginRecieve(this);
        }

        public void Send(Element e, Action<Element> error)
        {
            connection.Send(parser.ToBytes(e), error != null ? _ => error(e) : (Action<byte[]>)null);
        }

        public void Close()
        {
            connection.Close();
        }

        public void Reset()
        {
            parser.Reset();
        }


        void IXmppReciever.OnRecive(byte[] buffer)
        {
            parser.ParseAsync(buffer);
        }

        void IXmppReciever.OnClose()
        {
            handlerManager.ProcessClose(this);
        }
    }
}
