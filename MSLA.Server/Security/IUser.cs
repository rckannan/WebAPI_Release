using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    /// <summary>
    /// This interface is to be implemented by the class that holds the 
    /// authenticated session with the server.
    /// </summary>
    public interface IUser
    {
        /// <summary>Gets the user ID</summary>
        long User_ID { get; }
        /// <summary>Gets the Logon User Name</summary>
        string LogonName { get; }
        /// <summary>Gets the Full User Name</summary>
        string FullUserName { get; }
        /// <summary>Gets whether the user is SuperUser</summary>
        bool IsSuperUser { get; }      
        /// <summary>Gets the Session GUID</summary>
        Guid Session_ID { get; }       
    }
}
