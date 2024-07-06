using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;

namespace Mashreq.BO
{
    [Serializable]
    public class Customer1
       : MSLA.Server.BO.MasterBase
    {
        private String _fldName = "";
        private Int64 _fldAccount_ID = -1;
        private tblCustomerDetail _tblCustomerDetail;
        private DataTable _tblCustomer;
        private decimal _fldAmt = 0;
        [NonSerialized]
        private DataView _dvCustomerDetail = null;
        private EnType _fldEnType = EnType.Local;
        private DateTime _fldDate = DateTime.Today;

        tblRoleAccessLevel _dtDocMaster;


        #region "Public Enum"
        public enum EnType
        {
            Local = 1,
            Foreign = 2,
            Alien = 3
        }
        #endregion

        public virtual tblCustomerDetail TableOf_tblCustomerDetail
        {
            get { return _tblCustomerDetail; }            
        }


        public virtual DataTable tblCustomer
        {
            get { return _tblCustomer; }
            set { _tblCustomer = value; }
        }

        public virtual DataView ViewOf_tblCustomerDetail
        {
            get
            {
                if (_dvCustomerDetail == null)
                { _dvCustomerDetail = new DataView(_tblCustomerDetail); }
                return _dvCustomerDetail;
            }
        }

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

        public EnType fldEnType
        {
            get { return _fldEnType; }
            set { _fldEnType = value; }
        }

        public decimal fldAmt
        {
            get { return _fldAmt; }
            set { _fldAmt = value; }
        }

        public DateTime fldDate
        {
            get { return _fldDate; }
            set { _fldDate = value; }
        }
        #region "Overloaded Constructor"

        protected Customer1(MSLA.Server.Security.IUser MyUserInfo)
            : base(MyUserInfo)
        {
            _tblCustomerDetail = new tblCustomerDetail();
            _tblCustomer = new DataTable();
            _dtDocMaster = new tblRoleAccessLevel();
        }

        #endregion

        protected override void FetchOrCreate(MSLA.Server.BO.IMasterCriteria MastCriteria)
        {
            SqlCommand cmm = new SqlCommand();
            DataTable dt = new DataTable();
            cmm = new SqlCommand();

            if (MastCriteria.DocMaster_ID == -1)
            {                

                //if (MastCriteria.PropertyCollection["RefCust_ID"] != null)
                //{
                //    cmm.CommandType = CommandType.StoredProcedure;
                //    cmm.CommandText = "dbo.spCustomerFetch";
                //    cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt).Value = Convert.ToInt64(MastCriteria.PropertyCollection["RefCust_ID"]);
                //    dt = MSLA.Server.Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);
                //    if (dt.Rows.Count == 1)
                //    {
                //        this.fldName = Convert.ToString(dt.Rows[0]["fldName"]);
                //        this.fldEnType = (EnType)Enum.ToObject(typeof(EnType), (dt.Rows[0]["fldEnType"]));
                //    }

                //    cmm = new SqlCommand();
                //    cmm.CommandType = CommandType.StoredProcedure;
                //    cmm.CommandText = "dbo.spCustomerDetailFetch";
                //    cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt).Value = MastCriteria.DocMaster_ID;
                //    _tblCustomerDetail = (tblCustomerDetail)(MSLA.Server.Data.DataConnect.FillDt(_tblCustomerDetail, cmm, this.UserInfo, MSLA.Server.Data.DBConnectionType.CompanyDB));

                //    DataTable dt1 = MSLA.Server.Data.DataConnect.ResolveToSystemTable(MSLA.Server.Data.DataConnect.ResolveToSimpleTable(_tblCustomerDetail));
                //}

                //MSLA.Server.Utilities.NotificationMailWorker NotifyMail = new MSLA.Server.Utilities.NotificationMailWorker();
                //NotifyMail.fldBCc = "vish.priyanka@gmail.com";
                //NotifyMail.fldBody = "Test xls Body";
                //NotifyMail.fldCategory_ID = 1;
                //NotifyMail.fldCc = "ashiya2020@gmail.com,priyanka@vishwayon.com";
                //NotifyMail.fldFileName = "New OpenDocument Spreadsheet.ods";
                //NotifyMail.fldMailFrom = "devdatta@vishwayon.com";
                //NotifyMail.fldMailTo = "priyankas3@hotmail.com";
                //NotifyMail.fldSubject = "Test xls";

                //System.IO.Stream fileStream = System.IO.File.OpenRead("D:\\New OpenDocument Spreadsheet.ods");

                //byte[] byteArr = new byte[fileStream.Length];
                //fileStream.Read(byteArr, 0, Convert.ToInt32(fileStream.Length - 1));
                ////lstFileName.Items.Add(str.Name);
                //fileStream.Flush();
                //fileStream.Close();
                //NotifyMail.fldAttachment = byteArr;
                //NotifyMail.SaveNotificationMail(this.UserInfo);
            }
            else
            {
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "dbo.spCustomerFetch";
                cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt).Value = MastCriteria.DocMaster_ID;
                dt = MSLA.Server.Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);
                if (dt.Rows.Count == 1)
                {
                    this.SetDocMaster_ID(Convert.ToInt64(dt.Rows[0]["fldAccount_ID"]));
                    this.fldName = Convert.ToString(dt.Rows[0]["fldName"]);
                    this.fldEnType = (EnType)Enum.ToObject(typeof(EnType), (dt.Rows[0]["fldEnType"]));
                    this.fldAmt = Convert.ToDecimal(dt.Rows[0]["fldAmt"]);
                    this.fldDate = Convert.ToDateTime(dt.Rows[0]["fldDate"]);
                }

                cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "dbo.spCustomerDetailFetch";
                cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt).Value = MastCriteria.DocMaster_ID;
                _tblCustomerDetail = (tblCustomerDetail)(MSLA.Server.Data.DataConnect.FillDt(_tblCustomerDetail, cmm, this.UserInfo, MSLA.Server.Data.DBConnectionType.CompanyDB));

                DataTable dt1 = MSLA.Server.Data.DataConnect.ResolveToSystemTable(MSLA.Server.Data.DataConnect.ResolveToSimpleTable(_tblCustomerDetail));
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
            //Server.Rules.BrokenRule brRule;
            //if (this.fldAccount_ID == -1)
            //{
            //    base.BrokenSaveRules.Add(this.ToString(), this.ToString(), "Account No cannot be -1.");
            //}

            if (this.fldName == string.Empty)
            {
                base.BrokenSaveRules.Add(this.ToString(), this.ToString(), "Name cannot be blank.");
            }


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
                cmm.Parameters.Add("@Amt", SqlDbType.Decimal, 0).Value = this.fldAmt;
                cmm.Parameters.Add("@EnType", SqlDbType.SmallInt, 0).Value = this.fldEnType;
                cmm.Parameters.Add("@Date", SqlDbType.DateTime).Value = this.fldDate.ToString(MSLA.Server.Constants.SQLDateFormat);
                //MSLA.Server.Data.DataConnect.ExecCMM(
                cmm.ExecuteNonQuery();

                
            }

            MSLA.Server.Base.MasterSaveBase.MasterSaveResult SaveResult = new MSLA.Server.Base.MasterSaveBase.MasterSaveResult();
            {
                SaveResult.fldMasterDoc_ID = Convert.ToInt64(cmm.Parameters["@Account_ID"].Value);
            }



            //********************** Save For PV GLCredit Tran
            cmm = new SqlCommand();
            cmm.Connection = cn;
            cmm.CommandText = "dbo.spCustomerDetailDelete";
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.Transaction = cnTran;
            cmm.Parameters.Add("@Account_ID", SqlDbType.VarChar, 50);
            cmm.Parameters["@Account_ID"].Value = SaveResult.fldMasterDoc_ID;

            cmm.ExecuteNonQuery();

            cmm = new SqlCommand();
            cmm.Connection = cn;
            cmm.CommandText = "dbo.spCustomerDetailAdd";
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.Transaction = cnTran;
            cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt, 0);
            cmm.Parameters.Add("@Age", SqlDbType.BigInt, 0);
            cmm.Parameters.Add("@Email", SqlDbType.VarChar, 50);

            foreach (DataRow  drCustomer in this.TableOf_tblCustomerDetail.Rows)
            {
                cmm.Parameters["@Account_ID"].Value = SaveResult.fldMasterDoc_ID;
                cmm.Parameters["@Email"].Value = drCustomer["fldEmail"];
                cmm.Parameters["@Age"].Value = drCustomer["fldAge"];
                cmm.Parameters["@Account_ID"].Value = drCustomer["fldAccount_ID"];

                cmm.ExecuteNonQuery();
            }
            return SaveResult;
        }

        protected override void AfterSave(MSLA.Server.Base.MasterSaveBase.MasterSaveResult LocalSaveResult)
        {

        }

        protected override void ValidateBeforeDelete()
        {
            SqlCommand cmm=new SqlCommand();
        }

        protected override MSLA.Server.Base.MasterSaveBase.MasterSaveResult DeleteControlTran(System.Data.SqlClient.SqlConnection cn, System.Data.SqlClient.SqlTransaction cnTran)
        {
            SqlCommand cmm = new SqlCommand();
            {
                cmm.Connection = cn;
                cmm.Transaction = cnTran;
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "dbo.spCustomerDelete";
                cmm.Parameters.Add("@Account_ID", SqlDbType.BigInt, 0).Value = this.fldAccount_ID;
                cmm.ExecuteNonQuery();
            }
            return new MSLA.Server.Base.MasterSaveBase.MasterSaveResult(Convert.ToInt64(this.fldAccount_ID));
        }

        #region "Customer Criteria"
        [Serializable()]
        public class CustomerCriteria : MSLA.Server.BO.IMasterCriteria
        {
            private long _fldAccount_ID = -1;
            private string _DocMasterType = "Customer";
            private Dictionary<string, object> _PropertyCollection = new Dictionary<string, object>();

            /// <summary>Use this constructor to edit group </summary>
            /// <param name="Account_ID">The Group ID to Edit</param>
            public CustomerCriteria(long Account_ID)
            {
                _fldAccount_ID = Account_ID;
            }

            long MSLA.Server.BO.IMasterCriteria.DocMaster_ID
            {
                get { return _fldAccount_ID; }
            }

            public virtual string DocMasterType
            {
                get { return _DocMasterType; }
            }

            public Dictionary<string, object> PropertyCollection
            {
                get { return _PropertyCollection; }
            }
        }
        #endregion

        [Serializable]
        public class tblRoleAccessLevel : DataTable, IEnumerable
        {
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblRoleAccessLevel()
                : base()
            {
                this.TableName = "tblRoleAccessLevel";
                this.Columns.Add("fldRoleAccessLevel_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldRole_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldBranch_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldDocObject_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldEnAccessLevel", Type.GetType("System.Int16")).DefaultValue = MSLA.Server.Security.EnAccessLevel.No_Access;
                this.Columns.Add("fldIsExclusive", Type.GetType("System.Boolean")).DefaultValue = false;


                //Custom Columns
                this.Columns.Add("fldDocDescription", Type.GetType("System.String"));
                this.Columns.Add("fldBranchName", Type.GetType("System.String"));
                this.Columns.Add("fldBranchDocObject_ID", Type.GetType("System.String"));
                //this.PrimaryKey = new DataColumn[] { this.Columns["fldBranchDocObject_ID"]};

            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected tblRoleAccessLevel(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {

            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public IEnumerator GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new tblRoleAccessLevelRow(builder);
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override Type GetRowType()
            {
                return typeof(tblRoleAccessLevelRow);
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblRoleAccessLevelRow this[int index]
            {
                get { return ((tblRoleAccessLevelRow)(this.Rows[index])); }
            }

            [Serializable]
            public class tblRoleAccessLevelRow : DataRow
            {
                [System.Diagnostics.DebuggerNonUserCodeAttribute]
                protected internal tblRoleAccessLevelRow(System.Data.DataRowBuilder rb)
                    : base(rb)
                {

                }
                public Int64 fldRoleAccessLevel_ID
                {
                    get { return ((Int64)(this["fldRoleAccessLevel_ID"])); }
                    set { this["fldRoleAccessLevel_ID"] = value; }
                }

                public Int64 fldRole_ID
                {
                    get { return ((Int64)(this["fldRole_ID"])); }
                    set { this["fldRole_ID"] = value; }
                }

                public Int64 fldBranch_ID
                {
                    get { return ((Int64)(this["fldBranch_ID"])); }
                    set { this["fldBranch_ID"] = value; }
                }

                public Int64 fldDocObject_ID
                {
                    get { return ((Int64)(this["fldDocObject_ID"])); }
                    set { this["fldDocObject_ID"] = value; }
                }

                public Int16 fldEnAccessLevel
                {
                    get { return ((Int16)(this["fldEnAccessLevel"])); }
                    set { this["fldEnAccessLevel"] = value; }
                }

                public Boolean fldIsExclusive
                {
                    get { return ((Boolean)(this["fldIsExclusive"])); }
                    set { this["fldIsExclusive"] = value; }
                }

                // Custom Property

                public String fldDocDescription
                {
                    get { return ((String)(this["fldDocDescription"])); }
                    set { this["fldDocDescription"] = value; }
                }
                public String fldBranchName
                {
                    get { return ((String)(this["fldBranchName"])); }
                    set { this["fldBranchName"] = value; }
                }

                public String fldBranchDocObject_ID
                {
                    get { return ((String)(this["fldBranchDocObject_ID"])); }
                    set { this["fldBranchDocObject_ID"] = value; }
                }
            }
        }

        [Serializable]
        public class tblCustomerDetail : DataTable, IEnumerable
        {
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblCustomerDetail()
                : base()
            {
                this.TableName = "tblCustomerDetail";
                this.Columns.Add("fldAccount_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldAge", Type.GetType("System.Int64"));
                this.Columns.Add("fldEmail", Type.GetType("System.String"));

            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected tblCustomerDetail(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {

            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public IEnumerator GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new tblCustomerDetailRow(builder);
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override Type GetRowType()
            {
                return typeof(tblCustomerDetailRow);
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblCustomerDetailRow this[int index]
            {
                get { return (tblCustomerDetailRow)this.Rows[index]; }
            }

            [Serializable]
            public class tblCustomerDetailRow : DataRow
            {
                [System.Diagnostics.DebuggerNonUserCodeAttribute]
                protected internal tblCustomerDetailRow(System.Data.DataRowBuilder rb)
                    : base(rb)
                {

                }
                public Int64 fldAccount_ID
                {
                    get { return (Int64)this["fldAccount_ID"]; }
                    set { this["fldAccount_ID"] = value; }
                }

                public Int64 fldAge
                {
                    get { return (Int64)this["fldAge"]; }
                    set { this["fldAge"] = value; }
                }

                public String fldEmail
                {
                    get { return (String)this["fldEmail"]; }
                    set { this["fldEmail"] = value; }
                }


            }
        }

    }
}