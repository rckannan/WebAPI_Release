using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Exceptions
{
    /// <summary>An exception that is generated if Access Rights are violated</summary>
    [Serializable()]
    public class AccessRightsException
        : Exception
    {
        /// <summary>Constructor</summary>
        /// <param name="msg">The message to be sent</param>
        public AccessRightsException(string msg)
            : base(msg)
        {

        }

        /// <summary>Constructor</summary>
        /// <param name="info">The Serialization Info</param>
        /// <param name="context">The Streaming Context</param>
        public AccessRightsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
