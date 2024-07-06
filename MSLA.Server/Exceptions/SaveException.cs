using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Exceptions
{
    /// <summary>An Exception that is raised when a Document fails to save</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class SaveException
        : Exception, ICustomException
    {
        /// <summary>Constructor</summary>
        /// <param name="message">The Exception message</param>
        public SaveException(String message)
            : base(message)
        {
        }

        /// <summary>The Serialization Constructor</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected SaveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        string ICustomException.CustomMessage
        {
            get { return this.Message; }
        }
    }
}
