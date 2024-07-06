using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Validations
{
    /// <summary>This Exception is raised when the BO has broken Rules</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class ValidateException
        : Exception,
         Exceptions.ICustomException
    {
        /// <summary>
        /// Use this new constructor to raise an exception
        /// </summary>
        /// <param name="message">The message to be included in the exception</param>
        public ValidateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// This constructor is used by .Net Serialization
        /// </summary>
        /// <param name="info">The Serialization Info</param>
        /// <param name="context">The Serialization Context</param>
        protected ValidateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        string Exceptions.ICustomException.CustomMessage
        {
            get { return this.Message; }
        }
    }
}
