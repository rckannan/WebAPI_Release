using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    /// <summary>The Access Level Enum</summary>
    public enum EnAccessLevel
    {
        /// <summary>No Access</summary>
        No_Access = 0,
        /// <summary>Read Only</summary>
        Read_Only = 1,
        /// <summary>Data Entry</summary>
        Data_Entry = 2,
        /// <summary>Approval</summary>
        Approval = 3,
        /// <summary>Authorise</summary>
        Authorise = 4
    }

    /// <summary>The Master Item Access Level Enum</summary>
    [Flags()]
    [System.Runtime.Serialization.DataContract]
    public enum EnAccessLevelMaster
    {
        /// <summary>No Access</summary>
        No_Access = 0,
        /// <summary>Read Only</summary>
        Read_Only = 1,
        /// <summary>Create and Edit</summary>
        Create_Edit = 2,
    }
}
