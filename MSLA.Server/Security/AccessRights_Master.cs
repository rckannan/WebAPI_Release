using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Security
{
    public partial class AccessRights
    {
        /// <summary>Asserts the Access Rights for a Master Item</summary>
        /// <param name="MastDoc"></param>
        /// <param name="UserInfo"></param>
        public void Assert(IARMaster MastDoc, IUser UserInfo)
        {
            UserAccessMast UsrAcc;
            UsrAcc = GetAccessRight(MastDoc, UserInfo);

            if (UsrAcc.AccessLevel == EnAccessLevelMaster.No_Access)
            { throw new Exceptions.AccessRightsException("Access Level does not permit the logged user to access this master item."); }

            MastDoc.SetAccessLevel(UsrAcc.AccessLevel);
            MastDoc.SetIsDeleteAllowed(UsrAcc.AllowDelete);
        }

        private struct UserAccessMast
        {
            public EnAccessLevelMaster AccessLevel;
            public Boolean AllowDelete;
        }

        private UserAccessMast GetAccessRight(IARMaster MastDoc, IUser UserInfo)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.SpUserAccessLevelMasterFetch";

            cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = MastDoc.DocObjectInfo.DocObject_ID;
            cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = UserInfo.User_ID;
            cmm.Parameters.Add("@EnAccessLevelMaster", SqlDbType.Int).Value = 0;
            cmm.Parameters["@EnAccessLevelMaster"].Direction = ParameterDirection.InputOutput;
            cmm.Parameters.Add("@AllowDelete", SqlDbType.Bit).Value = 0;
            cmm.Parameters["@AllowDelete"].Direction = ParameterDirection.InputOutput;

            _DALocal.ExecCMM(cmm, UserInfo, Data.DBConnectionType.CompanyDB);

            UserAccessMast UA;

            UA.AccessLevel = (EnAccessLevelMaster)Enum.ToObject(typeof(EnAccessLevelMaster), Convert.ToInt32(cmm.Parameters["@EnAccessLevelMaster"].Value));
            UA.AllowDelete = (bool)cmm.Parameters["@AllowDelete"].Value;

            return UA;
        }

    }
}
