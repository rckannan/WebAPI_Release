using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MSLA.Server.Security;
using System.Runtime.Serialization;

namespace MSLA.Server.Base
{
    /// <summary>Abstract Workflow Base. The workflow logic is written in this class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract partial class WorkflowBase
        : VersionBase, Security.IAccessRight, Security.IARDocument, Security.IARCustomWorkflow
    {
        #region "private variables"
        private EnDocStatus _NextStatus = EnDocStatus.Unknown;
        private String _NextDesc = String.Empty;
        private bool _IsNextAllowed = false;
        private EnDocStatus _PreviousStatus = EnDocStatus.Unknown;
        private String _PreviousDesc = String.Empty;
        private bool _IsPreviousAllowed = false;
        private String _CurrentStatusDesc = String.Empty;

        private bool _IsSaveAllowed = false;
        private string _SaveNotAllowedReason = "Document defaults to 'save not allowed'.";
        private bool _IsPosted = false;
        private bool _IsUnpostAllowed = false;
        private bool _IsDeleteAllowed = false;
        private Decimal _MaxAllowedValue = 0;

        private Security.EnAccessLevel _AccessLevel;

        internal Base.DocObject _DocObjectInfo;
        private EnDocFlowLevel _DocFlowLevel;
        private tblDocWorkflowLevel _RoutingTable;
        private tblDocFieldAccess _FieldAccessTable;
        private tblUserWithRole _UserWithRoleTable;

        /// <summary>Set the Client Machine Name to be saved into the Log</summary>
        public String ClientMachineName = String.Empty;

        /// <summary>The Entered User Name</summary>
        protected String _fldEnteredBy = String.Empty;
        /// <summary>The Entered User Logon Name</summary>
        protected String _fldEnteredUser = String.Empty;
        /// <summary>Contains the date of Entry</summary>
        protected String _fldEnteredOn = String.Empty;
        /// <summary>The Approved By User Name</summary>
        protected String _fldApprovedBy = String.Empty;
        /// <summary>The Approved User Logon Name</summary>
        protected String _fldApprovedUser = String.Empty;
        /// <summary>Contains the Date of Approval</summary>
        protected String _fldApprovedOn = String.Empty;
        /// <summary>The Authorised By User Name</summary>
        protected String _fldAuthorisedBy = String.Empty;
        /// <summary>Authorised User Logon Name</summary>
        protected String _fldAuthorisedUser = String.Empty;
        /// <summary>Contains the Date of Authorisation</summary>
        protected String _fldAuthorisedOn = String.Empty;
        /// <summary>Contains Custom Workflow User Details</summary>
        protected DataTable _dtCustomWFStatusHistory;

        #endregion
        /// <summary>Constructor</summary>
        /// <param name="User">User Info</param>
        protected WorkflowBase(Security.IUser User, MSLA.Server.Data.DBConnectionType DbType)
            : base(User, DbType)
        {

        }


        #region "Voucher_ID and Current Status "

        /// <summary>Object Info as mentioned in tblDocObjects</summary>
        public Base.DocObject DocObjectInfo
        {
            get { return _DocObjectInfo; }
        }

        /// <summary>The Doc Flow Level</summary>
        public Security.EnDocFlowLevel DocFlowLevel
        {
            get { return _DocFlowLevel; }
        }

        void IARDocument.SetDocFlowLevel(EnDocFlowLevel FlowLevel)
        {
            _DocFlowLevel = FlowLevel;
        }

        /// <summary>The Voucher ID/Document ID</summary>
        public abstract string fldVoucher_ID
        { get; }

        /// <summary>Set the Voucher ID</summary>
        /// <param name="Vch_ID">Voucher ID</param>
        protected abstract void SetVoucher_ID(String Vch_ID);

        EnDocStatus IARDocument.CurrentStatus
        {
            get { return fldStatus; }
        }

        /// <summary>Current Status of the Document</summary>
        public abstract EnDocStatus fldStatus
        { get; }

        /// <summary>Current Status Desc.</summary>
        public String CurrentStatusDesc
        {
            get { return _CurrentStatusDesc; }
        }

        void IARDocument.SetCurrentStatusDesc(string CStatusDesc)
        {
            this.SetCurrentStatusDesc(CStatusDesc);
        }

        /// <summary>Set Current Desc.</summary>
        /// <param name="CStatusDesc">Current Desc.</param>
        protected void SetCurrentStatusDesc(String CStatusDesc)
        {
            _CurrentStatusDesc = CStatusDesc;
        }

        /// <summary>Set Current Status</summary>
        /// <param name="NewStatus">Current Status/ New Status after save</param>
        protected abstract void SetCurrentStatus(EnDocStatus NewStatus);

        DocObjectBase IAccessRight.DocObjectInfo
        {
            get { return this.DocObjectInfo; }
        }

        /// <summary>The Company ID the Document belongs to</summary>
        public abstract long fldCompany_ID
        { get; }

        /// <summary>Set the Company ID</summary>
        /// <param name="Comp_ID">the Company ID</param>
        protected abstract void SetCompany_ID(long Comp_ID);

        /// <summary>The Branch ID the document belongs to</summary>
        public abstract long fldBranch_ID
        { get; }

        /// <summary>Set the Branch ID</summary>
        /// <param name="Branch_ID">The Branch ID</param>
        protected abstract void SetBranch_ID(long Branch_ID);

        /// <summary>Gets the Access Level of the User to the Document</summary>
        public Security.EnAccessLevel AccessLevel
        {
            get { return _AccessLevel; }
        }

        void IAccessRight.SetAccessLevel(Security.EnAccessLevel myAccessLevel)
        {
            _AccessLevel = myAccessLevel;
        }

        #endregion

        #region "Next Status"
        /// <summary>Gets the Next Status of the Document</summary>
        public EnDocStatus NextStatus
        {
            get { return _NextStatus; }
        }

        /// <summary>Gets True if Next is allowed </summary>
        public bool IsNextAllowed
        {
            get { return _IsNextAllowed; }
        }

        void IARDocument.SetNextStatus(EnDocStatus NewStatus)
        {
            _NextStatus = NewStatus;
        }

        void IARDocument.SetIsNextAllowed(bool NextAllowed)
        {
            _IsNextAllowed = NextAllowed;
        }

        #endregion

        #region "Previous Status"
        /// <summary>Gets the Previous Status</summary>
        public EnDocStatus PreviousStatus
        {
            get { return _PreviousStatus; }
        }

        /// <summary>Gets True if Previous is allowed</summary>
        public bool IsPreviousAllowed
        {
            get { return _IsPreviousAllowed; }
        }

        void IARDocument.SetPreviousStatus(EnDocStatus PrevStatus)
        {
            _PreviousStatus = PrevStatus;
        }

        void IARDocument.SetIsPreviousAllowed(bool PrevAllow)
        {
            _IsPreviousAllowed = PrevAllow;
        }

        #endregion

        #region "Save, Post, Delete"
        /// <summary>Gets True if Save is allowed</summary>
        public bool IsSaveAllowed
        {
            get
            {
                if (!_IsPosted)
                { return _IsSaveAllowed; }
                else
                { return false; }
            }
        }

        /// <summary>The reason for save not allowed for the logged user.</summary>
        public string SaveNotAllowedReason
        {
            get { return _SaveNotAllowedReason; }
        }

        void IARDocument.SetIsSaveAllowed(bool Allow, string reason)
        {
            _IsSaveAllowed = Allow;
            _SaveNotAllowedReason = reason;
        }

        /// <summary>Gets True if Document is posted</summary>
        public bool IsPosted
        {
            get { return _IsPosted; }
        }

        void IARDocument.SetIsPosted(bool valuePosted)
        {
            _IsPosted = valuePosted;
        }

        /// <summary>Gets True if Unpost is allowed</summary>
        public bool IsUnpostAllowed
        {
            get
            {
                if (_IsPosted)
                { return _IsUnpostAllowed; }
                else
                { return false; }
            }
        }

        void IARDocument.SetIsUnPostAllowed(bool Allow)
        {
            _IsUnpostAllowed = Allow;
        }

        /// <summary>Gets True if Delete is allowed</summary>
        public bool IsDeleteAllowed
        {
            get
            {
                if (!_IsPosted)
                {
                    return _IsDeleteAllowed;
                }
                else
                {
                    return false;
                }
            }
        }

        void IARDocument.SetIsDeleteAllowed(bool Allow)
        {
            _IsDeleteAllowed = Allow;
        }

        /// <summary>Gets the Maximum Allowed value of the document for the user to post</summary>
        public Decimal MaxAllowedValue
        {
            get
            {
                return _MaxAllowedValue;
            }
        }

        void IARDocument.SetMaxAllowedValue(Decimal MaxVal)
        {
            _MaxAllowedValue = MaxVal;
        }
        #endregion

        #region "Prev, Next Desc"
        /// <summary>Gets the Next Desc.</summary>
        public string NextDesc
        {
            get { return _NextDesc; }
        }

        void IARDocument.SetNextDesc(string DescVal)
        {
            _NextDesc = DescVal;
        }

        /// <summary>Gets the Previous Desc.</summary>
        public String PreviousDesc
        {
            get { return _PreviousDesc; }
        }

        void IARDocument.SetPreviousDesc(String PrevDesc)
        {
            _PreviousDesc = PrevDesc;
        }

        #endregion

        #region "Abstract Workflow User Info"
        /// <summary>Contains the username for Entered By</summary>
        public string fldEnteredBy
        { get { return _fldEnteredBy; } }

        /// <summary>The Logon User who Entered the document</summary>
        public string fldEnteredUser
        { get { return _fldEnteredUser; } }

        /// <summary>Contains the Entered On Date (in longdate format)</summary>
        public string fldEnteredOn
        { get { return _fldEnteredOn; } }

        /// <summary>Contains the username for Approved By</summary>
        public string fldApprovedBy
        { get { return _fldApprovedBy; } }

        /// <summary>The Logon User who Approved the Document</summary>
        public string fldApprovedUser
        { get { return _fldApprovedUser; } }

        /// <summary>Contains the Approved Date (in long format)</summary>
        public string fldApprovedOn
        { get { return _fldApprovedOn; } }

        /// <summary>Contains the username for Authorised By</summary>
        public string fldAuthorisedBy
        { get { return _fldAuthorisedBy; } }

        /// <summary>The Logon User who authorised the document</summary>
        public string fldAuthorisedUser
        { get { return _fldAuthorisedUser; } }

        /// <summary>Contains the Authorisation Date (in long format)</summary>
        public string fldAuthorisedOn
        { get { return _fldAuthorisedOn; } }

        /// <summary>Sets the Entered By Field. Use this in AfterSave Method</summary>        
        protected void SetEnteredBy(String UName)
        { this._fldEnteredBy = UName; }

        /// <summary>Sets the Logon Name of the Entered User</summary>
        /// <param name="UName">User Name</param>
        protected void SetEnteredUser(String UName)
        { this._fldEnteredUser = UName; }

        /// <summary>Sets the Entered On</summary>
        /// <param name="EDate">Pass Date or DBNull</param>
        protected void SetEnteredOn(object EDate)
        {
            if (EDate == System.DBNull.Value)
            { _fldEnteredOn = String.Empty; }
            else
            { _fldEnteredOn = Convert.ToDateTime(EDate).ToString("dd MMM, yyyy HH:mm:ss"); }
        }

        /// <summary>Sets the Approved By Field. Use this in AfterSave Method</summary>
        protected void SetApprovedBy(String UName)
        { this._fldApprovedBy = UName; }

        /// <summary>Sets the Logon Name of the Approved User</summary>
        /// <param name="UName">User Name</param>
        protected void SetApprovedUser(String UName)
        { this._fldApprovedUser = UName; }

        /// <summary>Sets the Approved On</summary>
        /// <param name="EDate">Pass Date or DBNull</param>
        protected void SetApprovedOn(object EDate)
        {
            if (EDate == System.DBNull.Value)
            { _fldApprovedOn = String.Empty; }
            else
            { _fldApprovedOn = Convert.ToDateTime(EDate).ToString("dd MMM, yyyy HH:mm:ss"); }
        }

        /// <summary>Sets the Authorised By Field. Use this in AfterSave Method</summary>        
        protected void SetAuthorisedBy(String UName)
        { this._fldAuthorisedBy = UName; }

        /// <summary>Sets the Logon Name of the Authorised User</summary>
        /// <param name="UName">User Name</param>
        protected void SetAuthorisedUser(String UName)
        { this._fldAuthorisedUser = UName; }

        /// <summary>Sets the Authorised On</summary>
        /// <param name="EDate">Pass Date or DBNull</param>
        protected void SetAuthorisedOn(object EDate)
        {
            if (EDate == System.DBNull.Value)
            { _fldAuthorisedOn = String.Empty; }
            else
            { _fldAuthorisedOn = Convert.ToDateTime(EDate).ToString("dd MMM, yyyy HH:mm:ss"); }
        }

        /// <summary>Sets the Custom Workflow User Details</summary>
        /// <param name="dtWFStatusHistory"></param>
        protected internal void SetCustomWFDetails(DataTable dtWFStatusHistory)
        {
            _dtCustomWFStatusHistory = dtWFStatusHistory;
        }

        /// <summary>Contains the Custom Workflow User Details</summary>
        public DataTable fldCustomWFStatusHistory
        {
            get
            {
                if (_dtCustomWFStatusHistory == null)
                {
                    CreateDT();
                }

                return _dtCustomWFStatusHistory;
            }
        }

        private void CreateDT()
        {
            _dtCustomWFStatusHistory = new DataTable();
            _dtCustomWFStatusHistory.Columns.Add("fldApproveStep_ID", Type.GetType("System.Int32")).DefaultValue = 0;
            _dtCustomWFStatusHistory.Columns.Add("fldFullUserName", Type.GetType("System.String")).DefaultValue = String.Empty;
            _dtCustomWFStatusHistory.Columns.Add("fldLastUpdated", Type.GetType("System.String")).DefaultValue = String.Empty;
        }

        #endregion

        #region "Custom Workflow Routing Table"
        /// <summary>Gets the Routing Table. This is tblDocWorkflow. This table contains the custom workflow information.</summary>
        [VersionNotRequired]
        public tblDocWorkflowLevel RoutingTable
        {
            get
            {
                if (_DocFlowLevel == EnDocFlowLevel.CustomFlow)
                {
                    return _RoutingTable;
                }
                else
                {
                    throw new Exception("Document is not set to Custom Workflow. No Routing Table available.");
                }
            }
        }

        /// <summary>Gets the Field Access Table. This table contains the fields that have restriced access for each Approve Step of the document.</summary>
        [VersionNotRequired]
        public tblDocFieldAccess FieldAccessTable
        {
            get
            {
                if (_DocFlowLevel == EnDocFlowLevel.CustomFlow)
                {
                    return _FieldAccessTable;
                }
                else
                {
                    throw new Exception("Document is not set to Custom Workflow. No Field Access Table available.");
                }
            }
        }

        /// <summary>Gets the User's Roles Table. This table contains Role(s) assigned to the connected User</summary>
        [VersionNotRequired]
        public tblUserWithRole UserWithRoleTable
        {
            get
            {
                if (_DocFlowLevel == EnDocFlowLevel.CustomFlow)
                {
                    return _UserWithRoleTable;
                }
                else
                {
                    throw new Exception("Document is not set to Custom Workflow. No User with Role Table available.");
                }
            }
        }

        internal void SetRoutingTable(tblDocWorkflowLevel dtRouting)
        {
            _RoutingTable = dtRouting;
        }

        internal void SetUserWithRoleTable(tblUserWithRole dtUserWithRole)
        {
            _UserWithRoleTable = dtUserWithRole;
        }

        #region IARCustomWorkflow Members

        #region "Private Variables for CustomWorkflow"
        private short _ApproveStep_ID = -1;
        private long _ApproveRole_ID = -1;
        private short _NextStep_ID = -1;
        private long _NextRole_ID = -1;
        private WFValidatorInfo _WFValidatorInfo;
        #endregion

        /// <summary>Gets the Approved Step ID</summary>
        public short ApproveStep_ID
        {
            get { return _ApproveStep_ID; }
        }

        void IARCustomWorkflow.SetApproveStep_ID(short ApprovedStep)
        {
            _ApproveStep_ID = ApprovedStep;
        }

        /// <summary>Gets the Role ID that approved this step.</summary>
        public long ApproveRole_ID
        {
            get { return _ApproveRole_ID; }
        }

        void IARCustomWorkflow.SetApproveRole_ID(long ApprovedRole)
        {
            _ApproveRole_ID = ApprovedRole;
        }

        /// <summary>Gets the next step ID as per the table Document Status</summary>
        public short NextStep_ID
        {
            get { return _NextStep_ID; }
        }

        void IARCustomWorkflow.SetNextStep_ID(short NextStep)
        {
            _NextStep_ID = NextStep;
        }

        /// <summary>Gets the Next Role ID that has to manage this document.</summary>
        public long NextRole_ID
        {
            get { return _NextRole_ID; }
        }

        void IARCustomWorkflow.SetNextRole_ID(long NextRole)
        {
            _NextRole_ID = NextRole;
        }

        /// <summary>Gets the WF Validator Class Info. Can be used in reflection to invoke the class dynamically.</summary>
        public WFValidatorInfo WFValidatorInfo
        {
            get { return _WFValidatorInfo; }
        }

        void IARCustomWorkflow.SetWFValidatorInfo(WFValidatorInfo CVI)
        {
            _WFValidatorInfo = CVI;
        }

        void IARCustomWorkflow.SetFieldAccess(tblDocFieldAccess dtAccess)
        {
            _FieldAccessTable = dtAccess;
        }

        #endregion

        #endregion

    }
    /// <summary>The Doc Status Enum</summary>
    public enum EnDocStatus
    {
        /// <summary>Status Unknown</summary>
        Unknown = -1,
        /// <summary>New or Pending</summary>
        NewOrPending = 0,
        /// <summary>Rejected on Approval</summary>
        RejOnApprove = 1,
        /// <summary>Waiting Approval</summary>
        WaitingApproval = 2,
        /// <summary>Rejected on Authorisation</summary>
        RejOnAuthorise = 3,
        /// <summary>Waiting Authorisation</summary>
        WaitingAuthorisation = 4,
        /// <summary>Authorised</summary>
        Authorised = 5
    }

    /// <summary>The Status Exception Class</summary>
    [Serializable()]
    public class StatusException
        : Exception
    {

        /// <summary>Constructor</summary>
        /// <param name="Msg">Message</param>
        public StatusException(String Msg)
            : base(Msg)
        {
        }

        /// <summary>Serialization Constructor</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public StatusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
