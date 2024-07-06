using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Base
{
    /// <summary>The Workflow Validator Class</summary>
    [Serializable()]
    public class WFValidatorBase
    {

        #region "Private Variables"
        private Security.IUser _UserInfo;

        #endregion


        #region "Constructor and Initialisation"

        /// <summary>Constructor</summary>
        /// <param name="User">UserInfo</param>
        public WFValidatorBase(Security.IUser User)
        {
            _UserInfo = User;
        }

        #endregion


        #region "Properties"

        /// <summary>Gets User Info</summary>
        protected Security.IUser UserInfo
        { get { return _UserInfo; } }


        #endregion

        internal void AssertAccess(WorkflowBase Doc)
        {
            if (Doc.RoutingTable == null)
            {   //  ****    Load Routing Table Information
                LoadDocumentWorkflow(Doc);
            }
            if (Doc.FieldAccessTable == null)
            {   // *****   Load the Field Access Information
                LoadFieldAccess(Doc);
            }

            if (Doc.UserWithRoleTable == null)
            {   // *****   Load the User's Role Information
                LoadUserWithRole(Doc);
            }

            //  ****    If the Document is not posted, then it should be available in workflow
            if (Doc.fldStatus != EnDocStatus.Authorised)
            {   //  ****    New Document is never in workflow
                if (Doc.fldStatus == EnDocStatus.NewOrPending && Doc.fldVoucher_ID == String.Empty)
                {
                    //  ****    Assert Access based on DataEntryRole
                    SetDataEntryAccess(Doc);
                }
                else // *** Document must be available in workflow
                {
                    SetApproveAccess(Doc);
                }
            }
            else
            {   //  ****    Fetch Workflow properties
                WFCustomSatus WFStatus = GetWFCustomStatus(Doc);
                ((Security.IARCustomWorkflow)Doc).SetApproveStep_ID(WFStatus.ApproveStep_ID);
                ((Security.IARCustomWorkflow)Doc).SetApproveRole_ID(WFStatus.ApproveRole_ID);
                SetDocAccessWF(Doc, false);
            }
        }

        private void SetDataEntryAccess(WorkflowBase Doc)
        {
            //  ****    Set the Custom Workflow properties
            Security.IARCustomWorkflow ARDoc = (Security.IARCustomWorkflow)Doc;
            ARDoc.SetApproveStep_ID(0);
            ARDoc.SetApproveRole_ID(Doc.RoutingTable.GetDataEntryRole());
            ARDoc.SetNextStep_ID(1);
            ARDoc.SetNextRole_ID(Doc.RoutingTable.GetDataEntryRole());

            //  ****    Verify whether the user belongs to the Data Entry Role
            if (UserInRole(Doc.RoutingTable.GetDataEntryRole()))
            {
                SetDocAccessWF(Doc, true);
            }
            else
            {
                SetDocAccessWF(Doc, false);
            }
        }

        private void SetApproveAccess(WorkflowBase Doc)
        {
            //  ****    Set the Custom Workflow properties
            Security.IARCustomWorkflow ARDoc = (Security.IARCustomWorkflow)Doc;
            WFCustomSatus WFStatus = GetWFCustomStatus(Doc);

            ARDoc.SetApproveStep_ID(WFStatus.ApproveStep_ID);
            ARDoc.SetApproveRole_ID(WFStatus.ApproveRole_ID);

            Int16 NextStep = GetNextStep_ID(Doc, WFStatus.ApproveStep_ID);
            ARDoc.SetNextStep_ID(NextStep);
            ARDoc.SetNextRole_ID(Doc.RoutingTable.GetStepRole(NextStep));

            //  ****    Verify whether the user belongs to the Data Entry Role
            if (UserInRole(ARDoc.NextRole_ID))
            {
                SetDocAccessWF(Doc, true);
            }
            else
            {
                SetDocAccessWF(Doc, false);
            }
        }

        private void SetDocAccessWF(WorkflowBase Doc, bool AllowAccess)
        {
            Security.IARDocument ARDoc = (Security.IARDocument)Doc;

            //  *** Set the Document workflow forward properties
            switch (ARDoc.CurrentStatus)
            {
                case Base.EnDocStatus.NewOrPending:
                    ARDoc.SetNextStatus(Base.EnDocStatus.WaitingApproval);
                    ARDoc.SetNextDesc("Send");
                    break;
                case EnDocStatus.RejOnApprove:
                    if (((Security.IARCustomWorkflow)ARDoc).ApproveStep_ID == 0)
                    {   //  Document is in Approve Workflow
                        ARDoc.SetNextStatus(Base.EnDocStatus.WaitingApproval);
                        ARDoc.SetNextDesc("Send");
                    }
                    else
                    {   //  Document is in Approve Workflow
                        ARDoc.SetNextStatus(Base.EnDocStatus.WaitingApproval);
                        ARDoc.SetNextDesc("Approve");
                    }
                    break;
                case Base.EnDocStatus.WaitingApproval:
                    //  ****    Set the Next Properties
                    if (Doc.RoutingTable.IsLastStep(GetNextStep_ID(Doc, Doc.ApproveStep_ID)))
                    {   //  ****    Document is on the last step, therefore authorise
                        ARDoc.SetNextStatus(Base.EnDocStatus.Authorised);
                        ARDoc.SetNextDesc("Authorise");
                    }
                    else
                    {   //  Document is in Approve Workflow
                        ARDoc.SetNextStatus(Base.EnDocStatus.WaitingApproval);
                        ARDoc.SetNextDesc("Approve");
                    }
                    break;
                case Base.EnDocStatus.WaitingAuthorisation:
                    ARDoc.SetNextStatus(Base.EnDocStatus.Authorised);
                    ARDoc.SetNextDesc("Authorise");
                    break;
                case Base.EnDocStatus.Authorised:
                    break;
            }

            //  ****    Set the Document Workflow backward properties
            switch (ARDoc.CurrentStatus)
            {
                case Base.EnDocStatus.NewOrPending:
                    ARDoc.SetPreviousStatus(Base.EnDocStatus.Unknown);
                    ARDoc.SetPreviousDesc("N.A.");
                    break;
                case EnDocStatus.RejOnApprove:
                    if (Doc.ApproveStep_ID != 1 || Doc.ApproveStep_ID != 0)
                    {
                        ARDoc.SetPreviousStatus(Base.EnDocStatus.RejOnApprove);
                        ARDoc.SetPreviousDesc("Reject");
                    }
                    break;
                case EnDocStatus.WaitingApproval:
                case EnDocStatus.WaitingAuthorisation:
                    ARDoc.SetPreviousStatus(Base.EnDocStatus.RejOnApprove);
                    ARDoc.SetPreviousDesc("Reject");
                    break;
                case EnDocStatus.Authorised:
                    ARDoc.SetPreviousStatus(Base.EnDocStatus.WaitingAuthorisation);
                    ARDoc.SetPreviousDesc("Unpost");
                    break;
            }

            //  ****    Set Document Status Description
            SetCurrentStatusDesc(Doc);

            //  ****    Restrict User Rights
            if (AllowAccess)
            {
                if (!Doc.IsPosted)
                {
                    ARDoc.SetIsSaveAllowed(true, String.Empty);
                    ARDoc.SetIsNextAllowed(true);
                    if (GetPreviousStep_ID(Doc, Doc.ApproveStep_ID) != -1)
                    {
                        ARDoc.SetIsPreviousAllowed(true);
                    }
                }
            }
            else
            {
                string Msg = string.Empty;
                if (ARDoc.CurrentStatus == EnDocStatus.Authorised)
                { Msg = String.Empty; }
                else
                { Msg = "Document locked by Custom Workflow.\n\r" + "Document awaits a user belonging to the Role - '" + GetRoleName(Doc.NextRole_ID) + "' for Approval."; }

                ARDoc.SetIsSaveAllowed(false, Msg);
                ARDoc.SetIsNextAllowed(false);
                ARDoc.SetIsPreviousAllowed(false);
            }


        }

        private void SetCurrentStatusDesc(Security.IARDocument doc)
        {
            //   ****    Set the description in txtStatus and Button Caption
            switch (doc.CurrentStatus)
            {
                case Base.EnDocStatus.NewOrPending:
                    if (doc.fldVoucher_ID == String.Empty)
                    { doc.SetCurrentStatusDesc("New"); }
                    else
                    { doc.SetCurrentStatusDesc("Pending"); }
                    break;
                case Base.EnDocStatus.RejOnApprove:
                    doc.SetCurrentStatusDesc("Rejected on Approval");
                    break;
                case Base.EnDocStatus.WaitingApproval:
                    if (((WorkflowBase)doc).NextStatus == EnDocStatus.Authorised)
                    {   //  ****    Document is on the last step, therefore waiting Authorisation
                        doc.SetCurrentStatusDesc("Waiting Authorisation");
                    }
                    else
                    {
                        doc.SetCurrentStatusDesc("Waiting Approval");
                    }
                    break;
                case Base.EnDocStatus.RejOnAuthorise:
                    doc.SetCurrentStatusDesc("Rejected on Authorisation");
                    break;
                case Base.EnDocStatus.WaitingAuthorisation:
                    doc.SetCurrentStatusDesc("Waiting Authorisation");
                    break;
                case Base.EnDocStatus.Authorised:
                    doc.SetCurrentStatusDesc("Authorised");
                    break;
                default:
                    doc.SetCurrentStatusDesc("Unknown");
                    break;
            }

        }

        private Int16 GetNextStep_ID(WorkflowBase Doc, Int16 CurrentStep)
        {
            DataRow[] dr = Doc.RoutingTable.Select("fldApproveStep_ID>" + CurrentStep.ToString(), "fldApproveStep_ID");
            if (dr.Length > 0) // The document can go to the next step
            {
                return GetForwardStep_ID(Doc, CurrentStep, Convert.ToInt16(dr[0]["fldApproveStep_ID"]));
            }
            else
            {
                return 99;
            }
        }

        /// <summary>Override this method to return the forward step based on conditional workflow</summary>
        /// <param name="Doc">The Doc Object</param>
        /// <param name="CurrentStep">The Step being queried for.</param>
        /// <param name="NewStep_ID">The forward step as resolved by the base.</param>
        /// <returns>The forward step id. (default is the new step id received in input parameter)</returns>
        protected virtual Int16 GetForwardStep_ID(WorkflowBase Doc, Int16 CurrentStep, Int16 NewStep_ID)
        {
            return NewStep_ID;
        }

        private Int16 GetPreviousStep_ID(WorkflowBase Doc, Int16 CurrentStep)
        {
            if (CurrentStep == 1) // If Doc is created and saved, the CurrentStep would be 1
            { return 0; }
            else
            {
                DataRow[] dr = Doc.RoutingTable.Select("fldApproveStep_ID<" + CurrentStep.ToString(), "fldApproveStep_ID Desc");
                if (dr.Length > 0) // The document can go to previous step
                {
                    return GetBackwardStep_ID(Doc, CurrentStep, Convert.ToInt16(dr[0]["fldApproveStep_ID"]));
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>Override this method to implement conditional workflow</summary>
        /// <param name="Doc">The Doc Object</param>
        /// <param name="CurrentStep">The Requested step</param>
        /// <param name="NewStep_ID">The resolved New Step by the base.</param>
        /// <returns></returns>
        protected virtual Int16 GetBackwardStep_ID(WorkflowBase Doc, Int16 CurrentStep, Int16 NewStep_ID)
        {
            return NewStep_ID;
        }

        /// <summary>Saves the Workflow in the Doc Object Status table</summary>
        /// <param name="cn">Open Connection</param>
        /// <param name="cnTran">Open Transaction</param>
        /// <param name="VchDetails">Vch Detail returned by Save Control</param>
        /// <param name="Doc">The Doc Object</param>
        /// <param name="DoAction">The action requested by the user.</param>
        protected internal virtual void SaveWorkflowStatus(SqlConnection cn, SqlTransaction cnTran, SaveResult VchDetails, SaveBase Doc, SaveBase.InnerWorkFlow DoAction)
        {
            if (DoAction == SaveBase.InnerWorkFlow.PushUp)
            {
                //  ****    First resolve the next step
                Int16 NextStep = GetNextStep_ID(Doc, Doc.ApproveStep_ID);
                Int64 NextRole = -1;
                if (NextStep != 99) // Authorisor would not have a next role
                { NextRole = Doc.RoutingTable.GetStepRole(NextStep); }

                if (NextStep == 99)
                {   //  ****    If it is the last step, then remove the document from workflow
                    SqlCommand Cmm = new SqlCommand();
                    Cmm.CommandType = CommandType.StoredProcedure;
                    Cmm.CommandText = "Workflow.spWFCustomStatusDelete";
                    Cmm.Connection = cn;
                    Cmm.Transaction = cnTran;

                    Cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = VchDetails.fldVoucher_ID;
                    Cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = Doc.DocObjectInfo.DocObjectType;

                    Cmm.ExecuteNonQuery();
                }
                else
                {   //  ****    Make a document entry in workflow
                    SqlCommand Cmm = new SqlCommand();
                    Cmm.CommandType = CommandType.StoredProcedure;
                    Cmm.CommandText = "Workflow.spWFCustomStatusAddUpdate";
                    Cmm.Connection = cn;
                    Cmm.Transaction = cnTran;

                    Cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = VchDetails.fldVoucher_ID;
                    Cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = Doc.DocObjectInfo.DocObjectType;
                    Cmm.Parameters.Add("@Company_ID", SqlDbType.BigInt, 0).Value = Doc.fldCompany_ID;
                    Cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt, 0).Value = Doc.fldBranch_ID;
                    Cmm.Parameters.Add("@ApproveStep_ID", SqlDbType.SmallInt, 0).Value = NextStep;
                    Cmm.Parameters.Add("@ApproveRole_ID", SqlDbType.BigInt, 0).Value = NextRole;
                    Cmm.Parameters.Add("@NextStep_ID", SqlDbType.SmallInt, 0).Value = GetNextStep_ID(Doc, NextStep); // Gets the Step after Next Step
                    Cmm.Parameters.Add("@NextRole_ID", SqlDbType.BigInt, 0).Value = Doc.RoutingTable.GetStepRole(GetNextStep_ID(Doc, NextStep)); // Gets the related role for Step after Next Step
                    Cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = Doc.UserInfo.User_ID;

                    Cmm.ExecuteNonQuery();
                }
            }
            else if (DoAction == SaveBase.InnerWorkFlow.PushDown)
            {
                //  ****    First resolve the previous step
                Int16 PreviousStep = GetPreviousStep_ID(Doc, Doc.ApproveStep_ID);
                Int64 PreviousRole = Doc.RoutingTable.GetStepRole(PreviousStep);

                //  ****    Make a document entry in workflow
                SqlCommand Cmm = new SqlCommand();
                Cmm.CommandType = CommandType.StoredProcedure;
                Cmm.CommandText = "Workflow.spWFCustomStatusAddUpdate";
                Cmm.Connection = cn;
                Cmm.Transaction = cnTran;

                Cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = VchDetails.fldVoucher_ID;
                Cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = Doc.DocObjectInfo.DocObjectType;
                Cmm.Parameters.Add("@Company_ID", SqlDbType.BigInt, 0).Value = Doc.fldCompany_ID;
                Cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt, 0).Value = Doc.fldBranch_ID;
                Cmm.Parameters.Add("@ApproveStep_ID", SqlDbType.SmallInt, 0).Value = PreviousStep;
                Cmm.Parameters.Add("@ApproveRole_ID", SqlDbType.BigInt, 0).Value = PreviousRole;
                Cmm.Parameters.Add("@NextStep_ID", SqlDbType.SmallInt, 0).Value = GetNextStep_ID(Doc, PreviousStep);
                Cmm.Parameters.Add("@NextRole_ID", SqlDbType.BigInt, 0).Value = Doc.RoutingTable.GetStepRole(GetNextStep_ID(Doc, PreviousStep));
                Cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = Doc.UserInfo.User_ID;

                Cmm.ExecuteNonQuery();
            }

        }

        private void LoadDocumentWorkflow(WorkflowBase Doc)
        {
            //  *** Set the Routing Table
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spRoutingTableFetch";

            Cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = Doc.DocObjectInfo.DocObject_ID;
            Cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = Doc.fldBranch_ID;

            Base.WorkflowBase.tblDocWorkflowLevel dtRouting = new WorkflowBase.tblDocWorkflowLevel();
            dtRouting = (WorkflowBase.tblDocWorkflowLevel)Data.DataConnect.FillDt( this.UserInfo, Cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);
            Doc.SetRoutingTable(dtRouting);
            if (dtRouting.Rows.Count == 0)
            {
                throw new Exception("Routing information for the document was not available on the server. Failed to resolve Custom Workflow.");
            }
        }

        private void LoadFieldAccess(WorkflowBase Doc)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spDocWorkflowAccessCollection";

            cmm.Parameters.Add("@Branch_ID", SqlDbType.BigInt).Value = Doc.fldBranch_ID;
            cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = Doc.DocObjectInfo.DocObject_ID;
            cmm.Parameters.Add("@ApproveStep_ID", SqlDbType.BigInt).Value = 0; // This would fetch all fields access

            Security.tblDocFieldAccess dtAccess = new MSLA.Server.Security.tblDocFieldAccess();
            dtAccess = (Security.tblDocFieldAccess)MSLA.Server.Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);
            ((Security.IARCustomWorkflow)Doc).SetFieldAccess(dtAccess);
        }

        private void LoadUserWithRole(WorkflowBase Doc)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spUserWithRoleFetch";

            cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = this.UserInfo.User_ID;
            WorkflowBase.tblUserWithRole dtUserWithRole = new WorkflowBase.tblUserWithRole();
            dtUserWithRole = (WorkflowBase.tblUserWithRole)Data.DataConnect.FillDt(this.UserInfo, cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);
            Doc.SetUserWithRoleTable(dtUserWithRole);
        }

        private bool UserInRole(Int64 Role_ID)
        {
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spUserInRole";

            Cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = this.UserInfo.User_ID;
            Cmm.Parameters.Add("@Role_ID", SqlDbType.BigInt).Value = Role_ID;
            Cmm.Parameters.Add("@IsInRole", SqlDbType.Bit).Value = false;
            Cmm.Parameters["@IsInRole"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM( this.UserInfo, ref Cmm,MSLA.Server.Data.DBConnectionType.CompanyDB);
            return Convert.ToBoolean(Cmm.Parameters["@IsInRole"].Value);
        }

        private WFCustomSatus GetWFCustomStatus(WorkflowBase Doc)
        {
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "Workflow.spWFCustomStatusFetch";

            Cmm.Parameters.Add("@Voucher_ID", SqlDbType.VarChar, 50).Value = Doc.fldVoucher_ID;
            Cmm.Parameters.Add("@DocObjectType", SqlDbType.VarChar, 50).Value = Doc.DocObjectInfo.DocObjectType;
            Cmm.Parameters.Add("@ApproveStep_ID", SqlDbType.BigInt).Value = -1;
            Cmm.Parameters["@ApproveStep_ID"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@ApproveRole_ID", SqlDbType.BigInt).Value = -1;
            Cmm.Parameters["@ApproveRole_ID"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@NextStep_ID", SqlDbType.BigInt).Value = -1;
            Cmm.Parameters["@NextStep_ID"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@NextRole_ID", SqlDbType.BigInt).Value = -1;
            Cmm.Parameters["@NextRole_ID"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(this.UserInfo, ref Cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);

            WFCustomSatus WFStatus = new WFCustomSatus();
            WFStatus.ApproveStep_ID = Convert.ToInt16(Cmm.Parameters["@ApproveStep_ID"].Value);
            WFStatus.ApproveRole_ID = Convert.ToInt64(Cmm.Parameters["@ApproveRole_ID"].Value);
            WFStatus.NextStep_ID = Convert.ToInt16(Cmm.Parameters["@NextStep_ID"].Value);
            WFStatus.NextRole_ID = Convert.ToInt64(Cmm.Parameters["@NextRole_ID"].Value);

            return WFStatus;
        }

        private string GetRoleName(Int64 Role_ID)
        {
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spRoleNameGet";

            Cmm.Parameters.Add("@Role_ID", SqlDbType.BigInt).Value = Role_ID;
            Cmm.Parameters.Add("@RoleName", SqlDbType.VarChar, 50).Value = string.Empty;
            Cmm.Parameters["@RoleName"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(UserInfo, ref Cmm, MSLA.Server.Data.DBConnectionType.CompanyDB);

            return Cmm.Parameters["@RoleName"].Value.ToString();

        }

        /// <summary>The WF Status in Doc Object Status</summary>
        public struct WFCustomSatus
        {
            /// <summary>The Approve Step completed by the document</summary>
            public Int16 ApproveStep_ID;
            /// <summary>The Role that Approved the document.</summary>
            public Int64 ApproveRole_ID;
            /// <summary>The next Step the document is waiting for.</summary>
            public Int16 NextStep_ID;
            /// <summary>The Next Role the document is waiting for.</summary>
            public Int64 NextRole_ID;
        }

    }
}
