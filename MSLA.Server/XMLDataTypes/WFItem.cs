using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.XMLDataTypes
{
    /// <summary>The Work Flow Item Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable]
    public class WFItem
    {
        /// <summary>The Voucher ID</summary>
        public String Voucher_ID = String.Empty;
        /// <summary>Doc Object Type</summary>
        public String DocObjectType = String.Empty;
        /// <summary>Doc Object Desc.</summary>
        public String Description = String.Empty;
        /// <summary>Doc Status</summary>
        public Base.EnDocStatus Status = Base.EnDocStatus.Unknown;
        /// <summary>The Branch ID</summary>
        public long Branch_ID = 0;

        /// <summary>Constructor</summary>
        public WFItem()
        {
        }

        /// <summary>Returns XML String of the WFItem</summary>
        public String GetXML()
        {
            Utilities.XMLWriter XMLw = new Utilities.XMLWriter("WFItem");
            XMLw.SetValue("Voucher_ID", this.Voucher_ID);
            XMLw.SetValue("DocObjectType", this.DocObjectType);
            XMLw.SetValue("Status", ((Int32)this.Status).ToString());
            XMLw.SetValue("Description", this.Description);
            XMLw.SetValue("Branch_ID", this.Branch_ID.ToString());
            return XMLw.GetXML;
        }

        /// <summary>Returns the WF Item in Bytes</summary>
        public Byte[] GetBytes()
        {
            Utilities.XMLWriter XMLw = new Utilities.XMLWriter("WFItem");
            XMLw.SetValue("Voucher_ID", this.Voucher_ID);
            XMLw.SetValue("DocObjectType", this.DocObjectType);
            XMLw.SetValue("Status", ((Int32)this.Status).ToString());
            XMLw.SetValue("Description", this.Description);
            XMLw.SetValue("Branch_ID", this.Branch_ID.ToString());
            return XMLw.GetBytes;
        }
    }
}
