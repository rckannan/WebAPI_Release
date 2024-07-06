using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Login
{
    /// <summary>The class used to send information of the Company requested by the user</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class LogonInfoCompany
    {
        internal LogonResult ReturnedLogonResult;
        /// <summary>The Company ID</summary>
        public readonly long Company_ID = 0;

        /// <summary>Constructor</summary>
        /// <param name="myLogonInfo">The Logon Info</param>
        /// <param name="CompID">The Company ID</param>
        public LogonInfoCompany(LogonResult myLogonInfo, long CompID)
        {
            ReturnedLogonResult = myLogonInfo;
            Company_ID = CompID;
        }
    }
}
