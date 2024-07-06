using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Utilities
{
    /// <summary>This is the Reflection Helper Exception</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class ReflectionHelperException
        : Exception, Exceptions.ICustomException
    {
        /// <summary>Constructor for Rthe Exception</summary>
        /// <param name="Msg">The Exception message</param>
        /// <param name="ex">The parent Exception if any.</param>
        public ReflectionHelperException(string Msg, Exception ex)
            : base(Msg, ex)
        {
        }

        /// <summary>Constructor required by Serialization</summary>
        /// <param name="info">The Serialization Info</param>
        /// <param name="context">The Streaming Context</param>
        protected ReflectionHelperException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        string Exceptions.ICustomException.CustomMessage
        {
            get
            {
                return exMessage(this);
            }
        }

        private string exMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return exMessage(ex.InnerException);
            }
            else
            {
                return "\r\n" + ex.Message;
            }
        }



    }
}
