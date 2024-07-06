using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Base
{
    /// <summary>Abstract Save Base. The Save logic is written in this class.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class SaveBase
        : WorkflowBase, Validations.IValidateUsingAttributes//, Allocation.IAllocCollection
    {

        /// <summary>An Enum for Saving a Document.</summary>
        public enum InnerWorkFlow
        {
            /// <summary>Pushes a document up one step in the workflow. Increases the Status by one level</summary>
            PushUp = 1,
            /// <summary>Does not change the status of the document</summary>
            AsIs = 0,
            /// <summary>Pushes a document down one step in the workflow. Decreases the Status by one level</summary>
            PushDown = -1,
        }

        //private Data.DBConnectionType _ConnectionType = MSLA.Server.Data.DBConnectionType.CompanyDB;
        //private Allocation.AllocationSaveCollection _Allocations;
        private List<Exception> _ConstructionExceptions;
        private Rules.BrokenRuleCollection _BrokenSaveRules = new Rules.BrokenRuleCollection();
        private Rules.BrokenRuleCollection _BrokenDeleteRules = new Rules.BrokenRuleCollection();
        private Rules.BrokenRuleCollection _OpenWarnings = new Rules.BrokenRuleCollection();
        private int _CommandTimeOut = 30;

        /// <summary> Get/Set the command time out to complete save operation. Minimum is 30. </summary>
        public int CommandTimeOut
        {
            get { return _CommandTimeOut; }
            set
            {
                if (value < 30)
                { _CommandTimeOut = 30; }
                else
                { _CommandTimeOut = value; }
            }
        }

        /// <summary>Use this constructor to connect to the requested Database</summary>
        /// <param name="User">The User Info</param>
        /// <param name="DbType">The DB Type</param>
        protected SaveBase(Security.IUser User, Data.DBConnectionType DbType)
            : base(User, DbType)
        {
            //_ConnectionType = DbType;
            //_Allocations = new Allocation.AllocationSaveCollection();
            _ConstructionExceptions = new List<Exception>();
        }

        /// <summary>The Collection of Allocations</summary>
        //public Allocation.AllocationSaveCollection Allocations
        //{
        //    get { return _Allocations; }
        //}

        /// <summary>Returns the List of Exception that occured while creating instance of BO</summary>
        public List<Exception> ConstructionExceptions
        {
            get { return _ConstructionExceptions; }
        }

        #region "FinYear, Month Abstract Properties"

        /// <summary>Gets the FinYear of the Document</summary>
        public abstract string fldYear
        { get; }

        /// <summary>Override this to set the FinYear</summary>
        /// <param name="FinYear">FinYear</param>
        protected abstract void SetYear(string FinYear);

        /// <summary>Gets the Document Date required for validations</summary>
        public abstract DateTime fldDate
        {
            get;
            set;
        }

        /// <summary>Gets the Month of the Document if the system is set for monthly renumbering</summary>
        public abstract string fldMonth
        { get; }

        /// <summary>Override this to set the Month of the document</summary>
        /// <param name="FinMonth">Month</param>
        protected abstract void SetMonth(string FinMonth);

        /// <summary>Override this method to set the Sequence Type</summary>
        public abstract string fldType
        {
            get;
            set;
        }



        #endregion

        #region "Save Code"
        internal void DoSaveValidations(InnerWorkFlow DoAction)
        {
            //   ****    Clear all Broken Rules
            this.BrokenSaveRules.Clear();
            this.BrokenDeleteRules.Clear();
            this.OpenWarnings.Clear();

            //   ****   First call UI Validations
            base.OnValidateUIOnSave(new ValidateUIOnSaveEventArgs(DoAction));

            //   ****    Call BaseValidations
            ValidateBase();

            //   ****    Now Call Client Validations
            ValidateBeforeSave();
            //   ****    Call Attribute Validations
            AttributeValidations();
            ////  *****   Validate Alloc Before Save
            //ValidateAllocBeforeSave();

            //  ****    If posting, validate before post
            if (this.NextStatus == EnDocStatus.Authorised && DoAction == InnerWorkFlow.PushUp)
            { ValidateBeforePost(); }

            //  ****    Raise exception if broken rules exist
            if (DoAction == InnerWorkFlow.AsIs)
            {  //   *****  Exclude warnings when saving the document as is.
                if (this._BrokenSaveRules.RuleCount > 0)
                { throw new Validations.ValidateException("Document has broken business rules. Please rectify before saving."); }
            }
            else if (DoAction == InnerWorkFlow.PushUp && this.NextStatus != EnDocStatus.Authorised)
            {   //   *****  Exclude warnings when saving the document is pushed up but is not being authorised.
                if (this._BrokenSaveRules.RuleCount > 0)
                { throw new Validations.ValidateException("Document has broken business rules. Please rectify before saving."); }
            }
            else if (DoAction == InnerWorkFlow.PushDown && this.fldStatus != EnDocStatus.Authorised)
            {   //   *****  Exclude warnings when the document is pushed down but is not authorised.
                if (this._BrokenSaveRules.RuleCount > 0)
                { throw new Validations.ValidateException("Document has broken business rules. Please rectify before saving."); }
            }
            else
            {   //   *****  Validate all rules in all other cases.
                if (this.HasBrokenSaveRules)
                { throw new Validations.ValidateException("Document has broken business rules. Please rectify before saving."); }
            }

        }

        internal void DoBeforeUnpostValidations()
        {
            //  ****    Clear all Broken Rules
            this.BrokenSaveRules.Clear();
            this.BrokenDeleteRules.Clear();
            this.OpenWarnings.Clear();

            //  ****    Call Validations
            ValidateBeforeUnpost();
            //ValidateAllocBeforeUnpost();

            //  ****    Raise exception if broken rules exist
            if (this.HasBrokenSaveRules)
            { throw new Validations.ValidateException("Document has broken business rules. Please rectify before Unpost/UnAuthorise."); }
        }


        /// <summary>Validates the Base for Date contraints</summary>
        protected virtual void ValidateBase()
        {
            //  ****    If in edit mode, ensure that document belongs to the same month
            if (this.fldMonth != String.Empty)
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "System.spDateRangeValidationMonth";
                cmm.CommandTimeout = _CommandTimeOut;

                cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = this.fldBranch_ID;
                cmm.Parameters.Add("@Month", SqlDbType.VarChar, 4).Value = this.fldMonth;
                cmm.Parameters.Add("@Date", SqlDbType.DateTime).Value = this.fldDate.ToString(Constants.SQLDateFormat);
                cmm.Parameters.Add("@SequenceTable", SqlDbType.VarChar, 150).Value = this.DocObjectInfo.SequenceTable;
                cmm.Parameters.Add("@SeqType", SqlDbType.VarChar, 4).Value = this.fldType;
                cmm.Parameters.Add("@IsValid", SqlDbType.Bit).Value = 0;
                cmm.Parameters["@IsValid"].Direction = ParameterDirection.InputOutput;

                Data.DataConnect.ExecCMM(UserInfo, ref cmm, ConnectionType);

                if (!Convert.ToBoolean(cmm.Parameters["@IsValid"].Value))
                {
                    Rules.BrokenRule BRule = new Rules.BrokenRule(this.ToString(), "", "The selected document date does not belong to the month it was originally created");
                    this.BrokenSaveRules.Add(BRule);
                }
            }

            //  ****    If in a closed period, do not allow for save of the document

            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spPeriodCloseAccessLevel";
            Cmm.CommandTimeout = _CommandTimeOut;

            Cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = this.DocObjectInfo.DocObject_ID;
            Cmm.Parameters.Add("@DocDate", SqlDbType.DateTime).Value = this.fldDate.ToString(MSLA.Server.Constants.SQLDateFormat);
            Cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = this.fldBranch_ID;
            Cmm.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = true;
            Cmm.Parameters["@IsClosed"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = this.UserInfo.User_ID;

            Data.DataConnect.ExecCMM(this.UserInfo,ref Cmm,  MSLA.Server.Data.DBConnectionType.CompanyDB);

            if (Convert.ToBoolean(Cmm.Parameters["@IsClosed"].Value))
            {
                Rules.BrokenRule BRule = new Rules.BrokenRule(this.ToString(), "", "The document either belongs to a closed period or the period has not yet been defined in the system.");
                this.BrokenSaveRules.Add(BRule);
            }

        }

        /// <summary>Does basic validations based upon the Attributes</summary>
        protected virtual void AttributeValidations()
        {
            Validations.ValidateUsingAttributes Validate = new Validations.ValidateUsingAttributes(this.UserInfo);
            Validate.ValidateObject((Validations.IValidateUsingAttributes)this);
        }

        /// <summary>Validations before Save</summary>
        protected abstract void ValidateBeforeSave();

        ///// <summary>Validate Allocations Before Save</summary>
        //protected void ValidateAllocBeforeSave()
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //    {
        //        Alloc.ValidateBeforeSave(_BrokenSaveRules);
        //    }
        //}

        /// <summary>Validate Before Post</summary>
        protected virtual void ValidateBeforePost()
        {

        }

        /// <summary>Validate before Unpost</summary>
        protected abstract void ValidateBeforeUnpost();

        ///// <summary>Validate Alloc Before Unpost</summary>
        //protected void ValidateAllocBeforeUnpost()
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //    {
        //        Alloc.ValidateBeforeUnpost(_BrokenSaveRules);
        //    }
        //}

        internal bool SaveDocument(InnerWorkFlow DoAction)
        {
            SqlTransaction cnTran = null;
            SaveResult localSaveResult = new SaveResult(this.fldVoucher_ID);
            ChangeStatusResult CSResult;
            CSResult.StatusAfterUpdate = EnDocStatus.Unknown;
            LogVoucher myLogDetails;

            using (SqlConnection cn = Data.DataAccess.GetCn(ConnectionType))
            {
                try
                {
                    //   ****    First Open the Connection and create a transaction
                    cn.Open();
                    cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    if (this.IsPosted && DoAction == InnerWorkFlow.PushDown)
                    {
                        //  ****    Change Status
                        CSResult = ChangeStatus(cn, cnTran, localSaveResult, DoAction);

                        //  ****    Stamp Electronic Signature
                        ChangeStatusES(cn, cnTran, localSaveResult, DoAction);

                        //  ****    Execute Custom WF Status
                        this.SaveCustomWFSatus(cn, cnTran, localSaveResult, DoAction);

                        //   ****    Fetch Log Entry Details
                        myLogDetails = this.MakeLogDetails(localSaveResult, EnLogAction.Unposted);
                    }
                    else
                    {
                        //  ****    Clear the document Instance Creation Control
                        this.ClearDocObjectIC(cn, cnTran);

                        //   ****    Save Vch Control And Tran
                        localSaveResult = this.SaveControlTran(cn, cnTran);

                        //   ****    Execute any inherited methods
                        this.SaveInherited(cn, cnTran, localSaveResult);

                        ////   ****    Execute any Allocation methods
                        //this.SaveAllocation(cn, cnTran, localSaveResult);

                        //  ****    Execute Custom WF Status
                        this.SaveCustomWFSatus(cn, cnTran, localSaveResult, DoAction);

                        //   ****    Execute Only if Up or Down is required
                        if (DoAction != InnerWorkFlow.AsIs | this.fldVoucher_ID == String.Empty)
                        {
                            //  ****    Change Status
                            CSResult = ChangeStatus(cn, cnTran, localSaveResult, DoAction);

                            //  ****    Stamp Electronic Signature
                            ChangeStatusES(cn, cnTran, localSaveResult, DoAction);

                            //   ****    Fetch Log Entry Details
                            if (CSResult.StatusAfterUpdate == EnDocStatus.Authorised)
                            { myLogDetails = this.MakeLogDetails(localSaveResult, EnLogAction.Authorised); }
                            else
                            { myLogDetails = this.MakeLogDetails(localSaveResult, ResolveLogAction(DoAction)); }
                        }
                        else
                        {
                            myLogDetails = this.MakeLogDetails(localSaveResult, ResolveLogAction(DoAction));
                        }
                    }

                    //  ****    Make Log Entry
                    this.CreateLogEntry(cn, cnTran, myLogDetails);

                    //  Commit Transaction
                    cnTran.Commit();
                    cnTran = null;

                    //  ****    Fetch Month
                    GetMonth();

                    //   ****    Always call aftersave
                    if (CSResult.StatusAfterUpdate != EnDocStatus.Unknown)
                    {
                        this.SetCurrentStatus(CSResult.StatusAfterUpdate);
                        this.GetWorkflowES(localSaveResult);
                    }

                    this.AfterSave(localSaveResult);
                    //this.AfterSaveAlloc(localSaveResult);

                    //  ****    Clear Old Version and Create present data version
                    this.ClearVersion();
                    this.CreateVersion();
                    this.CreateCurrentLogEntry();
                }
                catch (Exception ex)
                {
                    if (cnTran != null)
                    {
                        try
                        { cnTran.Rollback(); }
                        catch { }
                    }
                   // Exceptions.ServiceExceptionHandler.HandleException(UserInfo.User_ID, UserInfo.Session_ID.ToString(), ex);
                    throw ex;
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed) { cn.Close(); }
                }
            }
            return true;
        }

        private void ClearDocObjectIC(SqlConnection cn, SqlTransaction cnTran)
        {
            // This code prevents a new document from being saved a second time and creating a copy of itself in the server.
            // In the case of documents that have already been saved, the voucher ID would be present and 
            // it would only update the record. Therefore, there is no danger.
            if (this.fldVoucher_ID == string.Empty)
            {
                SqlCommand Cmm = new SqlCommand();
                Cmm.CommandType = CommandType.StoredProcedure;
                Cmm.CommandText = "System.spDocObjectICRemove";
                Cmm.Parameters.Add("@DocObjectIC_ID", SqlDbType.BigInt).Value = this.GetDocObjectIC_ID;
                Cmm.Connection = cn;
                Cmm.Transaction = cnTran;
                Cmm.ExecuteNonQuery();
            }
        }

        /// <summary>Override this method to save the Document</summary>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The Open Transaction</param>
        protected abstract SaveResult SaveControlTran(SqlConnection cn, SqlTransaction cnTran);

        /// <summary>Override this method to save the inherited class items</summary>
        /// <param name="cn">The open connection</param>
        /// <param name="cnTran">The open Transaction</param>
        /// <param name="VchDetails">The Save Result</param>
        protected virtual void SaveInherited(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails)
        {
            //   ****    The Inherited Class can write code here
        }

        ///// <summary>Saves Allocations</summary>
        ///// <param name="cn">The open connection</param>
        ///// <param name="cnTran">The open Transaction</param>
        ///// <param name="VchDetails">The Save Result</param>
        //protected virtual void SaveAllocation(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails)
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //    {
        //        Alloc.Save(cn, cnTran, VchDetails);
        //    }
        //}

        /// <summary>This method saves data into the Doc Object Status Table</summary>
        /// <param name="cn">Open Connection</param>
        /// <param name="cnTran">Open Transaction</param>
        /// <param name="VchDetails">The vchdetail returned by the Bo on Save Control</param>
        /// <param name="DoAction">The Action requested by the user</param>
        protected virtual void SaveCustomWFSatus(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails, InnerWorkFlow DoAction)
        {
            if (this.DocFlowLevel == MSLA.Server.Security.EnDocFlowLevel.CustomFlow)
            {
                if (this.WFValidatorInfo.ValidatorObject == "WFValidatorBase")
                {
                    WFValidatorBase WFVal = new WFValidatorBase(this.UserInfo);
                    WFVal.SaveWorkflowStatus(cn, cnTran, VchDetails, this, DoAction);
                }
                else
                {
                    //  Use reflection to create an instance and call the same.
                    Base.WFValidatorBase WFVal = (Base.WFValidatorBase)Utilities.ReflectionHelper.CreateObject(this.WFValidatorInfo.ValidatorAssembly,
                                                        this.WFValidatorInfo.ValidatorNameSpace,
                                                        this.WFValidatorInfo.ValidatorObject,
                                                        new object[] { UserInfo });
                    WFVal.SaveWorkflowStatus(cn, cnTran, VchDetails, this, DoAction);
                }
            }
        }

        /// <summary>This should return the text that will be included in Workflow Notifications (Use XML).</summary>
        protected abstract string GetChangeStatusDesc();

        /// <summary>Toggles the Voucher Status</summary>
        /// <param name="cn">The open Connection</param>
        /// <param name="cnTran">The open Transaction</param>
        /// <param name="VchDetails">The Save Result</param>
        /// <param name="DoAction">The Action to perform(Up, Down or asis)</param>
        protected virtual ChangeStatusResult ChangeStatus(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails, InnerWorkFlow DoAction)
        {
            EnDocStatus NewStatus = EnDocStatus.Unknown;

            //   ****    Resolve NewStatus
            if (DoAction == InnerWorkFlow.PushUp)
            {
                NewStatus = this.NextStatus;
            }
            else if (DoAction == InnerWorkFlow.PushDown)
            {
                NewStatus = (EnDocStatus)this.PreviousStatus;
            }
            else if (DoAction == InnerWorkFlow.AsIs)
            {
                NewStatus = (EnDocStatus)this.fldStatus;
            }

            //   ****    Prepare Queue Item
            XMLDataTypes.WFItem myWFItem = new XMLDataTypes.WFItem();
            myWFItem.Voucher_ID = VchDetails.fldVoucher_ID;
            myWFItem.DocObjectType = this.DocObjectInfo.DocObjectType;
            myWFItem.Description = Utilities.StringParser.EncodeXML(GetChangeStatusDesc());
            myWFItem.Status = NewStatus;
            myWFItem.Branch_ID = this.fldBranch_ID;

            //   ****    Execute stored Proc
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spStatusToggle";
            cmm.Connection = cn;
            cmm.CommandTimeout = _CommandTimeOut;
            cmm.Transaction = cnTran;

            cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = VchDetails.fldVoucher_ID;
            cmm.Parameters.Add("@VoucherField", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.VoucherField;
            cmm.Parameters.Add("@TableName", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.TableName;
            cmm.Parameters.Add("@NewStatus", SqlDbType.SmallInt, 0).Value = NewStatus;
            cmm.Parameters.Add("@Message", SqlDbType.NVarChar, 3000).Value = myWFItem.GetXML();
            cmm.Parameters.Add("@StatusAfterUpdate", SqlDbType.SmallInt, 0).Direction = ParameterDirection.Output;

            cmm.ExecuteNonQuery();
            ChangeStatusResult Result;
            Result.StatusAfterUpdate = (EnDocStatus)Enum.ToObject(typeof(EnDocStatus), Convert.ToInt32(cmm.Parameters["@StatusAfterUpdate"].Value));

            ////   ****    Call the Allocation ChangeStatus
            //this.ChangeAllocStatus(cn, cnTran, VchDetails, NewStatus);

            return Result;
        }

        /// <summary>This procedure stamps the record with an electronic signature of the user</summary>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The open Transaction</param>
        /// <param name="VchDetails">The Save Vch Result returned by SaveControl</param>
        /// <param name="DoAction">The Save action requested by the user.</param>
        protected virtual void ChangeStatusES(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails, InnerWorkFlow DoAction)
        {
            EnDocStatus NewStatus = EnDocStatus.Unknown;

            //   ****    Resolve NewStatus
            if (DoAction == InnerWorkFlow.PushUp)
            {
                NewStatus = this.NextStatus;
            }
            else if (DoAction == InnerWorkFlow.PushDown)
            {
                NewStatus = (EnDocStatus)this.PreviousStatus;
            }
            else if (DoAction == InnerWorkFlow.AsIs)
            {
                NewStatus = (EnDocStatus)this.fldStatus;
            }

            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spStatusUpdateES";
            Cmm.Connection = cn;
            Cmm.Transaction = cnTran;
            Cmm.CommandTimeout = _CommandTimeOut;

            Cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = VchDetails.fldVoucher_ID;
            Cmm.Parameters.Add("@VoucherField", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.VoucherField;
            Cmm.Parameters.Add("@TableName", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.TableName;
            if (this.fldVoucher_ID == String.Empty)
            { Cmm.Parameters.Add("@IsNew", SqlDbType.Bit, 0).Value = true; }
            else
            { Cmm.Parameters.Add("@IsNew", SqlDbType.Bit, 0).Value = false; }
            Cmm.Parameters.Add("@CurrentStatus", SqlDbType.SmallInt, 0).Value = (int)this.fldStatus;
            Cmm.Parameters.Add("@NewStatus", SqlDbType.SmallInt, 0).Value = (int)NewStatus;
            Cmm.Parameters.Add("@FullUserName", SqlDbType.VarChar, 50).Value = this.UserInfo.FullUserName;
            Cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = this.UserInfo.LogonName;

            Cmm.ExecuteNonQuery();

        }
        //private void ChangeAllocStatus(SqlConnection cn, SqlTransaction cnTran, SaveResult localSaveResult, EnDocStatus NewStatus)
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in this.Allocations)
        //    {
        //        Allocation.ChangeStatusCriteria AllocCriteria = new Allocation.ChangeStatusCriteria();
        //        AllocCriteria.ParentBO = this;
        //        AllocCriteria.VchResult = localSaveResult;
        //        AllocCriteria.NewStatus = NewStatus;
        //        Alloc.ChangeStatus(cn, cnTran, AllocCriteria);
        //    }
        //}


        /// <summary>Method is called after sucessful save.</summary>
        /// <param name="VchDetails">The Save Result retuned in SaveControlTran</param>
        protected abstract void AfterSave(SaveResult VchDetails);

        ///// <summary>This method is called After Save. It calls all the allocations After Save</summary>
        ///// <param name="VchDetails">The Save Result returned by Voucher Save</param>
        //protected virtual void AfterSaveAlloc(SaveResult VchDetails)
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //        Alloc.AfterSave(VchDetails);
        //}

        internal void AfterPostAction(Object Param)
        {

            using (SqlConnection cn = Data.DataAccess.GetCn(ConnectionType))
            {
                SaveResult Result = null;
                cn.Open();
                SqlTransaction cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    Result = ModifyAfterPost(cn, cnTran, Param);

                    //  Make Log Entry
                    LogVoucher myLogDetails = this.MakeLogDetails(Result, EnLogAction.AfterPostAction);
                    CreateLogEntry(cn, cnTran, myLogDetails);

                    //  Commit transaction
                    cnTran.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        cnTran.Rollback();
                    }
                    catch { }
                    //Exceptions.ServiceExceptionHandler.HandleException(UserInfo.User_ID, UserInfo.Session_ID.ToString(), ex);
                    throw ex;

                }

                AfterPostModified(Param, Result);

                //  ****    Clear Old Version and Create present data version
                this.ClearVersion();
                this.CreateVersion();
                this.CreateCurrentLogEntry();
            }
        }

        internal void DoValidateModifyAfterPost()
        {
            //  *** Conduct Validations
            this.BrokenSaveRules.Clear();
            ValidateModifyAfterPost();
            if (this.BrokenSaveRules.Count > 0)
            { throw new Validations.ValidateException("Document has broken business rules. Please rectify before proceeding."); }

        }

        /// <summary>Override this method to do Validations before ModifyAfterPost. Include validations in BrokenSaveRules</summary>
        protected virtual void ValidateModifyAfterPost()
        {

        }

        /// <summary>Override this method to conduct actions after save</summary>
        /// <param name="cn">The SQL Connection</param>
        /// <param name="cnTran">The SQL Transaction</param>
        /// <param name="Param">The Param object, passed in the save method</param>
        protected virtual SaveResult ModifyAfterPost(SqlConnection cn, SqlTransaction cnTran, Object Param)
        {
            return new SaveResult();
        }

        /// <summary>Override this to set the values returned by the Modify After Post</summary>
        /// <param name="Param">The Param sent for Save</param>
        /// <param name="Result">The Save Result</param>
        protected virtual void AfterPostModified(Object Param, SaveResult Result)
        {

        }

        #endregion

        #region "Delete Code"

        internal void DoDeleteValidations()
        {
            //  ****    Clear DeleteRules
            this.BrokenDeleteRules.Clear();

            //   ****    Validate Document Status and Access Rights
            if (this.IsPosted)
            {
                throw new Exception("Posted document cannot be deleted. Delete Failed.");
            }
            else
            {
                if (!this.IsDeleteAllowed)
                {
                    throw new Exception("Access Rights prohibit document delete. Delete Failed.");
                }
            }

            //  ****    Make a round trip to ensure that the present document status is the same as that on the server
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spVerifyStatus";

            cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = this.fldVoucher_ID;
            cmm.Parameters.Add("@VoucherField", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.VoucherField;
            cmm.Parameters.Add("@TableName", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.TableName;
            cmm.Parameters.Add("@Status", SqlDbType.SmallInt).Value = (Int16)this.fldStatus;
            cmm.Parameters.Add("@StatusChanged", SqlDbType.Bit).Value = true;
            cmm.Parameters["@StatusChanged"].Direction = ParameterDirection.InputOutput;

            MSLA.Server.Data.DataConnect.ExecCMM(this.UserInfo, ref cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);

            if ((bool)cmm.Parameters["@StatusChanged"].Value)
            { throw new Exception("Document status changed. Delete prohibited! Please close and open the document."); }

            //   ****    Validate for Broken Rules
            ValidateBeforeDelete();
            //ValidateAllocBeforeDelete();
            if (this.HasBrokenDeleteRules)
            { throw new Validations.ValidateException("Document has broken business rules. Please rectify before deleting."); }
        }

        /// <summary>Override this method to do validations before delete</summary>
        protected abstract void ValidateBeforeDelete();

        ///// <summary>Validates allocations before delete.</summary>
        //protected void ValidateAllocBeforeDelete()
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //        Alloc.ValidateBeforeDelete(_BrokenDeleteRules);
        //}


        internal bool DeleteDocument()
        {

            SqlTransaction cnTran = null;

            using (SqlConnection cn = Data.DataAccess.GetCn(ConnectionType))
            {
                try
                {
                    //   ****    First Open the Connection and create a transaction
                    cn.Open();
                    cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    ////   ****    Execute any Allocation methods
                    //this.DeleteAllocation(cn, cnTran, this.fldVoucher_ID);

                    //   ****    Call the overridden Delete Method
                    this.DeleteEntireDocument(cn, cnTran);

                    //   ****    Make Log Entry
                    LogVoucher myLogDetails = this.MakeLogDetails(new SaveResult(this.fldVoucher_ID), EnLogAction.Deleted);
                    this.CreateLogEntry(cn, cnTran, myLogDetails);

                    cnTran.Commit();
                    cnTran = null;
                }
                catch (Exception ex)
                {
                    if (cnTran != null)
                    {
                        try
                        { cnTran.Rollback(); }
                        catch { }
                    }
                    //Exceptions.ServiceExceptionHandler.HandleException(UserInfo.User_ID, UserInfo.Session_ID.ToString(), ex);
                    throw ex;
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed) { cn.Close(); }
                }
            }

            //  ****    Clear Old Version
            this.ClearVersion();

            return true;
        }

        /// <summary>Override this method to delete the Entire Document.</summary>
        /// <param name="cn">The Open Transaction</param>
        /// <param name="cnTran">The Open Transaction</param>
        protected abstract bool DeleteEntireDocument(SqlConnection cn, SqlTransaction cnTran);

        ///// <summary>This method calls the Delete of all Allocations</summary>
        ///// <param name="cn">The Open Connection</param>
        ///// <param name="cnTran">The Open Transaction</param>
        ///// <param name="VoucherID">The Voucher ID of the document</param>
        //protected virtual void DeleteAllocation(SqlConnection cn, SqlTransaction cnTran, String VoucherID)
        //{
        //    foreach (Allocation.AllocationSaveBase Alloc in _Allocations)
        //        Alloc.Delete(cn, cnTran);
        //}

        #endregion

        #region "Log Entry"

        /// <summary>This method will make a log entry into the database using the save connection transaction. If neccessary, override this method to create your own implementation.</summary>
        /// <param name="myLogDetails">The Log Details returned by 'MakeLogEntry'.</param>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The Open Transaction</param>
        protected virtual void CreateLogEntry(SqlConnection cn, SqlTransaction cnTran, LogVoucher myLogDetails)
        {
            if (this.AuditMode != EnAuditMode.NoTrail)
            {   //  Make Audit Trail in all other cases
                SqlCommand cmm = new SqlCommand();
                cmm.Connection = cn;
                cmm.Transaction = cnTran;
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "AuditTrail.spLogDocumentAdd";
                cmm.CommandTimeout = _CommandTimeOut;

                cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = myLogDetails.DocType;
                cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = myLogDetails.Voucher_ID;
                cmm.Parameters.Add("@EnLogAction", SqlDbType.TinyInt).Value = myLogDetails.Action;
                cmm.Parameters.Add("@MachineName", SqlDbType.VarChar, 50).Value = this.ClientMachineName;
                cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 250).Value = myLogDetails.UserName;
                cmm.Parameters.Add("@CustomActionDesc", SqlDbType.VarChar, 250).Value = myLogDetails.CustomActionDesc;
                if (this.HasVersion)
                {
                    //Byte[] XmlDoc = this.GetVersionInfo;                    
                    //cmm.Parameters.Add("@XmlDoc", SqlDbType.Xml, XmlDoc.Length).Value = System.Text.Encoding.ASCII.GetString(XmlDoc);
                    cmm.Parameters.Add("@XmlDoc", SqlDbType.Xml, 100).Value = DBNull.Value;
                    cmm.Parameters.Add("@Version_ID", SqlDbType.UniqueIdentifier).Value = this.GetVersionInfo;
                }

                cmm.ExecuteNonQuery();
            }
        }

        /// <summary>This method will make a log entry into the database for the current version of the document.</summary>
        protected virtual void CreateCurrentLogEntry()
        {
            try
            {
                if (this.AuditMode == EnAuditMode.Complete)
                {   //  Make Audit Trail in all other cases
                    SqlCommand cmm = new SqlCommand();
                    cmm.CommandType = CommandType.StoredProcedure;
                    cmm.CommandText = "AuditTrail.spLogDocumentAddCurrent";
                    cmm.CommandTimeout = _CommandTimeOut;

                    cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.DocObjectType;
                    cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = this.fldVoucher_ID;
                    cmm.Parameters.Add("@EnLogAction", SqlDbType.TinyInt).Value = 99;
                    cmm.Parameters.Add("@MachineName", SqlDbType.VarChar, 50).Value = this.ClientMachineName;
                    cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 250).Value = this.UserInfo.FullUserName;
                    cmm.Parameters.Add("@CustomActionDesc", SqlDbType.VarChar, 250).Value = "CurrentVersion";
                    //Byte[] XmlDoc = this.GetVersionInfo;
                    //cmm.Parameters.Add("@XmlDoc", SqlDbType.Xml, XmlDoc.Length).Value = System.Text.Encoding.ASCII.GetString(XmlDoc);  
                    cmm.Parameters.Add("@XmlDoc", SqlDbType.Xml, 100).Value = DBNull.Value;
                    cmm.Parameters.Add("@Version_ID", SqlDbType.UniqueIdentifier).Value = this.GetVersionInfo;

                    MSLA.Server.Data.DataConnect.ExecCMM(this.UserInfo, ref cmm, this.ConnectionType);
                }
            }
            catch { }
        }

        /// <summary>Override this method to make Custom Log Entry</summary>
        protected virtual LogVoucher MakeLogDetails(Base.SaveResult localSaveResult, EnLogAction Action)
        {
            LogVoucher LogInfo = new LogVoucher();
            LogInfo.DocType = this.DocObjectInfo.DocObjectType;
            LogInfo.Voucher_ID = localSaveResult.fldVoucher_ID;
            LogInfo.UserName = this.UserInfo.FullUserName;
            LogInfo.Action = Action;
            LogInfo.CustomActionDesc = Enum.GetName(typeof(EnLogAction), Action);
            return LogInfo;
        }

        /// <summary>Resolves the LogAction Enum</summary>
        /// <param name="DoAction">The Innerworkflow action</param>
        protected virtual EnLogAction ResolveLogAction(InnerWorkFlow DoAction)
        {
            switch (DoAction)
            {
                case InnerWorkFlow.AsIs:
                    if (this.fldVoucher_ID == String.Empty)
                    { return EnLogAction.Created; }
                    else
                    { return EnLogAction.SavedOrEdited; }
                case InnerWorkFlow.PushUp:
                    if (this.fldVoucher_ID == String.Empty)
                    { return EnLogAction.Created; } // This is the first time it is being saved
                    if (this.DocFlowLevel == MSLA.Server.Security.EnDocFlowLevel.CustomFlow && this.fldStatus == EnDocStatus.WaitingApproval)
                    { return EnLogAction.Approved; }
                    switch (this.NextStatus)
                    {
                        case EnDocStatus.WaitingApproval:
                            return EnLogAction.SentForApproval;
                        case EnDocStatus.WaitingAuthorisation:
                            return EnLogAction.Approved;
                        case EnDocStatus.Authorised:
                            return EnLogAction.Authorised;
                        default:
                            return EnLogAction.SavedOrEdited;
                    }
                case InnerWorkFlow.PushDown:
                    switch (this.PreviousStatus)
                    {
                        case EnDocStatus.RejOnApprove:
                            return EnLogAction.RejectedOnApproval;
                        case EnDocStatus.RejOnAuthorise:
                            return EnLogAction.RejectedOnAuthorise;
                        case EnDocStatus.WaitingApproval:
                        case EnDocStatus.NewOrPending:
                            return EnLogAction.Unposted;
                        default:
                            return EnLogAction.SavedOrEdited;
                    }
                default:
                    return EnLogAction.SavedOrEdited;
            }
        }
        #endregion

        #region "Helper Functions, Properties"
        /// <summary>Returns 'True' if the Master Object has broken any Save Rules.</summary>
        public bool HasBrokenSaveRules
        {
            get
            {
                if (this._BrokenSaveRules.Count > 0)
                { return true; }
                else
                { return false; }
            }
        }

        /// <summary>Returns 'True' if the Master Object has broken any Delete Rules.</summary>
        public bool HasBrokenDeleteRules
        {
            get
            {
                if (this._BrokenDeleteRules.Count > 0)
                { return true; }
                else
                { return false; }
            }
        }

        /// <summary>Returns 'True' if the Object has any Opening warning.</summary>
        public bool HasOpenWarnings
        {
            get
            {
                if (this._OpenWarnings.Count > 0)
                { return true; }
                else
                { return false; }
            }
        }

        /// <summary>Returns the Collection of Broken Save Rules.</summary>
        public override Rules.BrokenRuleCollection BrokenSaveRules
        {
            get { return _BrokenSaveRules; }
        }

        /// <summary>Returns the Collection of Broken Delete Rules.</summary>
        public override Rules.BrokenRuleCollection BrokenDeleteRules
        {
            get { return _BrokenDeleteRules; }
        }

        /// <summary>Returns the Collection of Open Warnings.</summary>
        public Rules.BrokenRuleCollection OpenWarnings
        {
            get { return _OpenWarnings; }
        }

        /// <summary>Returns the Current Date Time of the Server after appropriate validations for FinYear Constraints</summary>
        /// <param name="DocCriteria">The DocCriteria</param>
        protected DateTime GetCurrentDate(BO.IDocCriteria DocCriteria)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spCurrentDateFetch";
            cmm.CommandTimeout = _CommandTimeOut;

            cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = DocCriteria.Branch_ID;
            cmm.Parameters.Add("@Year", SqlDbType.VarChar, 4).Value = DocCriteria.FinYear;
            cmm.Parameters.Add("@CurrentDate", SqlDbType.DateTime).Value = DateTime.Today;
            cmm.Parameters["@CurrentDate"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(this.UserInfo, ref cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);

            return Convert.ToDateTime(cmm.Parameters["@CurrentDate"].Value);
        }

        /// <summary>Gets the Current Month and sets it to fldMonth</summary>
        protected virtual void GetMonth()
        {
            if (this.fldVoucher_ID == string.Empty)
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "System.spCurrentMonthFetch";
                cmm.CommandTimeout = _CommandTimeOut;

                cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = this.fldBranch_ID;
                cmm.Parameters.Add("@SequenceTable", SqlDbType.VarChar, 150).Value = DocObjectInfo.SequenceTable;
                cmm.Parameters.Add("@Month", SqlDbType.VarChar, 4).Value = String.Empty;
                cmm.Parameters["@Month"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@DocumentDate", SqlDbType.DateTime).Value = this.fldDate.ToString(Constants.SQLDateFormat);
                cmm.Parameters.Add("@SeqType", SqlDbType.VarChar, 4).Value = this.fldType;

                Data.DataConnect.ExecCMM(this.UserInfo,ref cmm,  MSLA.Server.Data.DBConnectionType.CompanyDB);

                this.SetMonth(cmm.Parameters["@Month"].Value.ToString());
            }
        }

        /// <summary>Gets the Company ID from the database for the Branch ID that is set in property 'fldBranch_ID'</summary>
        /// <returns>The Company ID</returns>
        protected virtual long GetCompanyID()
        {
            if (this.fldBranch_ID == -1)
            { throw new Exception("Branch ID was not set in the property fldBranch_ID. Failed to retreive Company ID"); }

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spBranchCompanyFetch";
            cmm.CommandTimeout = _CommandTimeOut;

            cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = this.fldBranch_ID;
            cmm.Parameters.Add("@Company_ID", SqlDbType.BigInt).Value = -1;
            cmm.Parameters["@Company_ID"].Direction = ParameterDirection.InputOutput;

            MSLA.Server.Data.DataConnect.ExecCMM(this.UserInfo,ref cmm,  MSLA.Server.Data.DBConnectionType.CompanyDB);
            return Convert.ToInt64(cmm.Parameters["@Company_ID"].Value);
        }

        /// <summary>This fetches the workflow updated data after save.</summary>
        /// <param name="localSaveResult">The Save Result</param>
        protected internal virtual void GetWorkflowES(SaveResult localSaveResult)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandTimeout = _CommandTimeOut;

            cmm.CommandText = "Select @EnteredBy=fldEnteredBy, @EnteredUser=fldEnteredUser, @EnteredOn=fldEnteredOn, " +
                                   " @ApprovedBy=fldApprovedBy, @ApprovedUser=fldApprovedUser, @ApprovedOn=fldApprovedOn, " +
                                   " @AuthorisedBy=fldAuthorisedBy, @AuthorisedUser=fldAuthorisedUser, @AuthorisedOn=fldAuthorisedOn " +
                                   " From " + this.DocObjectInfo.TableName + "ES Where " + this.DocObjectInfo.VoucherField + "=@Voucher_ID";

            cmm.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@EnteredBy"].Value = String.Empty;
            cmm.Parameters.Add("@EnteredUser", SqlDbType.VarChar, 20).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@EnteredUser"].Value = String.Empty;
            cmm.Parameters.Add("@EnteredOn", SqlDbType.DateTime).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@EnteredOn"].Value = System.DBNull.Value;
            cmm.Parameters.Add("@ApprovedBy", SqlDbType.VarChar, 50).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@ApprovedBy"].Value = String.Empty;
            cmm.Parameters.Add("@ApprovedUser", SqlDbType.VarChar, 20).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@ApprovedUser"].Value = String.Empty;
            cmm.Parameters.Add("@ApprovedOn", SqlDbType.DateTime).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@ApprovedOn"].Value = System.DBNull.Value;
            cmm.Parameters.Add("@AuthorisedBy", SqlDbType.VarChar, 50).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@AuthorisedBy"].Value = String.Empty;
            cmm.Parameters.Add("@AuthorisedUser", SqlDbType.VarChar, 20).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@AuthorisedUser"].Value = String.Empty;
            cmm.Parameters.Add("@AuthorisedOn", SqlDbType.DateTime).Direction = ParameterDirection.InputOutput;
            cmm.Parameters["@AuthorisedOn"].Value = System.DBNull.Value;
            cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = localSaveResult.fldVoucher_ID;

            Data.DataConnect.ExecCMM(this.UserInfo, ref cmm, this.ConnectionType);

            this.SetEnteredBy(cmm.Parameters["@EnteredBy"].Value.ToString());
            this.SetEnteredUser(cmm.Parameters["@EnteredUser"].Value.ToString());
            this.SetEnteredOn(cmm.Parameters["@EnteredOn"].Value);
            //  *****   Set Approved
            this.SetApprovedBy(cmm.Parameters["@ApprovedBy"].Value.ToString());
            this.SetApprovedUser(cmm.Parameters["@ApprovedUser"].Value.ToString());
            this.SetApprovedOn(cmm.Parameters["@ApprovedOn"].Value);
            //  *****   Set Authorised
            this.SetAuthorisedBy(cmm.Parameters["@AuthorisedBy"].Value.ToString());
            this.SetAuthorisedUser(cmm.Parameters["@AuthorisedUser"].Value.ToString());
            this.SetAuthorisedOn(cmm.Parameters["@AuthorisedOn"].Value);


            //Fetch Custom Workflow User Details
            cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandTimeout = _CommandTimeOut;
            cmm.CommandText = "Workflow.spWFCustomStatusHistory";
            cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = localSaveResult.fldVoucher_ID;
            cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = this.DocObjectInfo.DocObjectType;

            DataTable dtWFStatusHistory = new DataTable();
            dtWFStatusHistory = Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);

            cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandTimeout = _CommandTimeOut;
            cmm.CommandText = "Select fldUser_ID, fldFullUserName + ' (' + fldUserName + ')' As fldFullUserName from Main.tblUser";

            DataTable dtUsers = new DataTable();
            dtUsers = Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.MainDB);

            dtWFStatusHistory.Columns.Add("fldFullUserName", Type.GetType("System.String")).DefaultValue = String.Empty;

            foreach (DataRow dr in dtWFStatusHistory.Rows)
            {
                if (dtUsers.Select("fldUser_ID = " + dr["fldUser_ID"].ToString()).Length == 1)
                {
                    dr["fldFullUserName"] = dtUsers.Select("fldUser_ID = " + dr["fldUser_ID"].ToString())[0]["fldFullUserName"].ToString();
                }
            }

            //Set Custom Workflow User Details
            if (dtWFStatusHistory.Rows.Count > 0)
            {
                this.SetCustomWFDetails(dtWFStatusHistory);
            }
        }

        #endregion

    }

    /// <summary> Validate UI On Save EventArgs</summary>
    public class ValidateUIOnSaveEventArgs
        : System.EventArgs
    {
        /// <summary>The Save Action</summary>
        public Base.SaveBase.InnerWorkFlow Action;

        /// <summary> Constructor </summary>
        public ValidateUIOnSaveEventArgs(Base.SaveBase.InnerWorkFlow MyAction)
        {
            Action = MyAction;
        }
    }
}
