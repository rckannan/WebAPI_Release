using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.XMLDataTypes
{
    /// <summary>The Que Master Item Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable]
    public class MasterItem
    {
        /// <summary>Master Item ID</summary>
        public long MasterItem_ID;
        /// <summary>Doc Object Type</summary>
        public string DocObjectType;
        /// <summary>Table Name</summary>
        public string TableName;
        /// <summary>Action Performed (Add/Update/Delete)</summary>
        public int Action;
        /// <summary>Any Custom Description</summary>
        public string Description;
        /// <summary>The Company ID</summary>
        public long Company_ID;

        /// <summary>Constructor</summary>
        public MasterItem()
        {
        }

        /// <summary>Gets the XML representation of the Master Item</summary>
        /// <returns>String as XML</returns>
        public string GetXML()
        {
            StringBuilder SBuilder = new StringBuilder();
            SBuilder.Append("<MasterItem>");
            SBuilder.Append("<MasterItem_ID>" + MasterItem_ID.ToString() + "</MasterItem_ID>");
            SBuilder.Append("<DocObjectType>" + DocObjectType + "</DocObjectType>");
            SBuilder.Append("<TableName>" + TableName + "</TableName>");
            SBuilder.Append("<Action>" + Action.ToString() + "</Action>");
            SBuilder.Append("<Description>" + Description + "</Description>");
            SBuilder.Append("</MasterItem>");
            return SBuilder.ToString();
        }

        /// <summary>Gets the Bytes of the entire XML representation</summary>
        /// <returns>Bytes</returns>
        public byte[] GetBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes(GetXML());
        }


    }
}
