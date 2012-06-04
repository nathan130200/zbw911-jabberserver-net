﻿using System;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;

namespace Jabber.Net.Xmpp
{
    public class XmppElement : ICloneable
    {
        public Node Node
        {
            get;
            private set;
        }

        public XmppElement(Node node)
        {
            Node = node;
        }

        public override string ToString()
        {
            return Node.ToString();
        }

        public object Clone()
        {
            return new XmppElement((Node)Node.Clone());
        }
    }
}
