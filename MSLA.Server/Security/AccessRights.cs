using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Security
{
    /// <summary>Teh Access Rights Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public partial class AccessRights
    {
        private Data.DataAccess _DALocal = new Data.DataAccess();

        private UserAccess GetAccessRight(IAccessRight Doc, Security.IUser User)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.SpUserAccessLevelFetch";

            cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = Doc.DocObjectInfo.DocObject_ID;
            cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = User.User_ID;
            cmm.Parameters.Add("@EnAccessLevel", SqlDbType.Int).Value = 0;
            cmm.Parameters["@EnAccessLevel"].Direction = ParameterDirection.InputOutput;
            cmm.Parameters.Add("@AllowUnpost", SqlDbType.Bit).Value = 0;
            cmm.Parameters["@AllowUnpost"].Direction = ParameterDirection.InputOutput;
            cmm.Parameters.Add("@AllowDelete", SqlDbType.Bit).Value = 0;
            cmm.Parameters["@AllowDelete"].Direction = ParameterDirection.InputOutput;
            cmm.Parameters.Add("@MaxAllowedValue", SqlDbType.Decimal, 0).Value = 0;
            cmm.Parameters["@MaxAllowedValue"].Direction = ParameterDirection.Output;
            cmm.Parameters["@MaxAllowedValue"].Scale = 2;

            _DALocal.ExecCMM(cmm, User, Data.DBConnectionType.CompanyDB);

            UserAccess UA;

            UA.AccessLevel = (EnAccessLevel)Enum.ToObject(typeof(EnAccessLevel), Convert.ToInt32(cmm.Parameters["@EnAccessLevel"].Value));
            UA.AllowUnpost = (bool)cmm.Parameters["@AllowUnpost"].Value;
            UA.AllowDelete = (bool)cmm.Parameters["@AllowDelete"].Value;
            UA.MaxAllowedValue = (Decimal)cmm.Parameters["@MaxAllowedValue"].Value;

            return UA;
        }

        private struct UserAccess
        {
            public EnAccessLevel AccessLevel;
            public bool AllowUnpost;
            public bool AllowDelete;
            public Decimal MaxAllowedValue;
        }
    }
}
