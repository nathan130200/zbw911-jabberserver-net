// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="Stream.cs">
//   
// </copyright>
// <summary>
//   (c) Copyright Ascensio System Limited 2008-2009
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using agsXMPP.protocol.stream;

namespace agsXMPP.protocol.client
{
    /// <summary>
    /// stream:stream Element
    /// This is the first Element we receive from the server.
    /// It encloses our whole xmpp session.
    /// </summary>
    public class Stream : Base.Stream
    {
        public Features Features
        {
            get { return this.SelectSingleElement<Features>(); }
            set { AddChild(value); }
        }

        #region Constructor

        /// <summary>
        /// </summary>
        public Stream()
        {
            Namespace = Uri.STREAM;
        }

        public Stream(Base.Stream stream, string defaultNamespace)
            : base(stream, defaultNamespace)
        {
        }

        #endregion        
    }
}