using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.BO
{
    /// <summary>The Master Criteria required to create instances of MasterBase. Implement this Interface and create Custom Serializable Criteria.</summary>
    public interface IMasterCriteria
    {
        /// <summary>The Master ID</summary>
        long DocMaster_ID
        { get; }
        /// <summary>The Master Doc Type</summary>
        string DocMasterType
        { get; }

        Dictionary<string, object> PropertyCollection
        { get; }
    }
}
