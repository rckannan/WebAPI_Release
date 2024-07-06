using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Data
{
    public enum DBConnectionType
    {
        /// <summary>Main Database</summary>
        MainDB = 0,
        /// <summary>Company Database</summary>
        CompanyDB = 1,
        /// <summary>DMS Database</summary>
        CompanyDMSDB = 2,
        /// <summary>OLTP Database </summary>
        OLTPDB = 3
    }

    public enum EnDataCommandType
    {
        Text = 1,
        StoredProcedure = 4,
        TableDirect = 512
    }

}
