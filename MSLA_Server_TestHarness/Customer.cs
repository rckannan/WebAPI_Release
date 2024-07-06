using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA_Server_TestHarness
{
    public class Customer
       : MSLA.Server.BO.MasterBase
    {
        private String _fldName = "";
        private Int64 _fldAccount_ID = -1;

        public String fldName
        {
            get { return _fldName; }
            set { _fldName = value; }
        }

        public Int64 fldAccount_ID
        {
            get { return _fldAccount_ID; }
            set { _fldAccount_ID = value; }
        }


        #region "Overloaded Constructor"

        protected Customer(MSLA.Server.Security.IUser MyUserInfo)
            : base(MyUserInfo)
        {
            //InitialiseMe();
        }

        #endregion

        protected override void FetchOrCreate(MSLA.Server.BO.IMasterCriteria MastCriteria)
        {
            SqlCommand cmm = new SqlCommand();
            DataTable dt = new DataTable();

            if (MastCriteria.DocMaster_ID == -1)
            {

            }
            else
            {
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "dbo.spCustomerFetch";
                cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt).Value = MastCriteria.DocMaster_ID;
                dt = MSLA.Server.Data.DataConnect.FillDt( this.UserInfo, cmm,MSLA.Server.Data.DBConnectionType.CompanyDB);
                if (dt.Rows.Count == 1)
                {
                    this.SetDocMaster_ID(Convert.ToInt64(dt.Rows[0]["fldAccount_ID"]));
                    this.fldName = Convert.ToString(dt.Rows[0]["fldName"]);
                    //this.fldAccount_ID=Convert.ToInt64(dt.Rows[0]["fldAccount_ID"]);
                }

            }

        }

        protected override void SetDocMaster_ID(long Item_ID)
        {
            this._fldAccount_ID = Item_ID;
        }

        protected override string MasterTableName
        {
            get { return "dbo.tblCustomer"; }
        }

        protected override void ValidateBeforeSave()
        {
            throw new NotImplementedException();
        }

        protected override MSLA.Server.Base.MasterSaveBase.MasterSaveResult SaveControlTran(System.Data.SqlClient.SqlConnection cn, System.Data.SqlClient.SqlTransaction cnTran)
        {
            SqlCommand cmm;
            cmm = new SqlCommand();
            {
                cmm.Connection = cn;
                cmm.Transaction = cnTran;
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "dbo.spCustomerAddUpdate";
                cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt, 0).Value = this.fldAccount_ID;
                cmm.Parameters["@Account_ID"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = this.fldName;
                cmm.ExecuteNonQuery();
            }

            MSLA.Server.Base.MasterSaveBase.MasterSaveResult SaveResult = new MSLA.Server.Base.MasterSaveBase.MasterSaveResult();
            {
                SaveResult.fldMasterDoc_ID = Convert.ToInt64(cmm.Parameters["@Account_ID"].Value);
                SaveResult.fldLastUpdated = Convert.ToDateTime(cmm.Parameters["@LastUpdated"].Value);
            }
            return SaveResult;
        }

        protected override void AfterSave(MSLA.Server.Base.MasterSaveBase.MasterSaveResult LocalSaveResult)
        {
            throw new NotImplementedException();
        }

        protected override void ValidateBeforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override MSLA.Server.Base.MasterSaveBase.MasterSaveResult DeleteControlTran(System.Data.SqlClient.SqlConnection cn, System.Data.SqlClient.SqlTransaction cnTran)
        {
            throw new NotImplementedException();
        }
    }
}