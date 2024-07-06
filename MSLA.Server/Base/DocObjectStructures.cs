using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MSLA.Server.Base
{
    /// <summary>Base Class for DocObject</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class DocObjectBase
    {
        /// <summary>The DocObject ID</summary>
        public long DocObject_ID;
        /// <summary>Assembly</summary>
        public string DocAssembly;
        /// <summary>Namespace</summary>
        public string DocNameSpace;
        /// <summary>Object Name</summary>
        public string DocObjectName;
        /// <summary>Object Description</summary>
        public string DocDescription;
        /// <summary>The DocObjectType</summary>
        public abstract string DocObjectType
        {
            get;
            set;
        }
    }


    /// <summary>Document Master Structure</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DocMaster
        : DocObjectBase
    {
        private string _DocObjectType = string.Empty;
        /// <summary>The Doc Master Type</summary>
        public override string DocObjectType
        {
            get { return _DocObjectType; }
            set { _DocObjectType = value; }
        }
    }

    /// <summary>Document Object Structure</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DocObject
        : DocObjectBase
    {
        private string _DocObjectType = string.Empty;
        /// <summary>The Doc Master Type</summary>
        public override string DocObjectType
        {
            get { return _DocObjectType; }
            set { _DocObjectType = value; }
        }
        /// <summary>Voucher Field</summary>
        public string VoucherField;
        /// <summary>Control Table Name</summary>
        public string TableName;
        /// <summary>Sequence Table Name</summary>
        public string SequenceTable;
    }
}
