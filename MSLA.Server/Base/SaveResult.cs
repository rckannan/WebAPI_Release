using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Base
{
    /// <summary>The Save Result that is to be returned after Save</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class SaveResult
    {
        /// <summary>The Voucher ID</summary>
        public string fldVoucher_ID = String.Empty;
        /// <summary>Last Updated returned after save</summary>
        public DateTime fldLastUpdated = DateTime.Now;

        /// <summary>Constructor</summary>
        public SaveResult()
            : this(String.Empty)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="VchNo">The Voucher No</param>
        public SaveResult(String VchNo)
        {
            this.fldVoucher_ID = VchNo;
        }
    }
}
