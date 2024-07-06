using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Login
{
    /// <summary>The Logon Exception</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class LogonException
        : Exception
    {
        /// <summary>Constructor</summary>
        /// <param name="ex">The generated exception</param>
        public LogonException(Exception ex)
            : base("Login Failed", ex)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public LogonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
