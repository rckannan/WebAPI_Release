using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.BO
{
    /// <summary>Criteria Interface required to be implemented by all Document Criteria</summary>
    public interface IDocCriteria
    {
        /// <summary>The ID of the Document. Usually the Primary Key. Set a blank value for a new document.</summary>
        string Voucher_ID
        { get; }
        /// <summary>The Doc Object Type. As is mentioned in System.tblDocObjects</summary>
        string DocObjectType
        { get; }
        /// <summary>The Branch ID for which the document needs to be created.</summary>
        long Branch_ID
        { get; }
        /// <summary>The Financial Year for which this document would be created.</summary>
        string FinYear
        { get; }
    }
}
