using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    /// <summary>Access Right Interface to be implemented by every document</summary>
    public interface IARDocument
        : IAccessRight
    {
        /// <summary>Return the Document ID/Voucher ID</summary>
        string fldVoucher_ID { get; }
        /// <summary>Set the Document Current Status</summary>
        Base.EnDocStatus CurrentStatus { get; }
        /// <summary>Gets the Document Access level</summary>
        EnAccessLevel AccessLevel { get; }
        /// <summary>Gets if Delete is allowed</summary>
        bool IsDeleteAllowed { get; }
        /// <summary>Gets if Unpost Is allowed</summary>
        bool IsUnpostAllowed { get; }
        /// <summary>Gets the Max Value of the document for the user</summary>
        Decimal MaxAllowedValue { get; }

        /// <summary>Set Save Allowed</summary>
        /// <param name="SaveAllowed">True for allowed</param>
        /// <param name="reason">Reason for not allowing save.</param>
        void SetIsSaveAllowed(bool SaveAllowed, string reason);
        /// <summary>Set if Delete is allowed</summary>
        /// <param name="DelAllowed">True if delete is allowed</param>
        void SetIsDeleteAllowed(bool DelAllowed);
        /// <summary>Set if Unpost is allowed</summary>
        /// <param name="UnPostAllowed">True if Unpost allowed</param>
        void SetIsUnPostAllowed(bool UnPostAllowed);
        /// <summary>Set the Max Value of the document for the user</summary>
        /// <param name = "MaxAllowedValue">If 0 then unlimited value and GT 0 is user can post to the max</param>
        void SetMaxAllowedValue(Decimal MaxAllowedValue);

        /// <summary>Set if Previous is allowed</summary>
        /// <param name="PrevAllowed">True if previous is allowed</param>
        void SetIsPreviousAllowed(bool PrevAllowed);
        /// <summary>Set the Previous Status as per workflow</summary>
        /// <param name="PrevStatus">The Status for one step back</param>
        void SetPreviousStatus(Base.EnDocStatus PrevStatus);
        /// <summary>Set the Previous Desc.</summary>
        /// <param name="PrevDesc">Previous Desc.</param>
        void SetPreviousDesc(String PrevDesc);

        /// <summary>Set if Next is allowed</summary>
        /// <param name="NextAllowed">True if next is allowed</param>
        void SetIsNextAllowed(bool NextAllowed);
        /// <summary>Set Next Status</summary>
        /// <param name="NextStatus">The Next Status</param>
        void SetNextStatus(Base.EnDocStatus NextStatus);
        /// <summary>Set Next Description</summary>
        /// <param name="NextDesc">The next Description</param>
        void SetNextDesc(String NextDesc);

        /// <summary>Set is Posted</summary>
        /// <param name="DocPosted">True if Posted</param>
        void SetIsPosted(bool DocPosted);
        /// <summary>Set Current Status Desc.</summary>
        /// <param name="CStatusDesc">Current Status Desc.</param>
        void SetCurrentStatusDesc(string CStatusDesc);

        /// <summary>Set Document Flow Level</summary>
        /// <param name="DocFlow">The Doc Flow Level</param>
        void SetDocFlowLevel(EnDocFlowLevel DocFlow);
    }

    /// <summary>Interface required by Custom Workflow Manager</summary>
    public interface IARCustomWorkflow
    {
        /// <summary>Gets the Approved Step that the Document has cleared.</summary>
        Int16 ApproveStep_ID { get; }
        /// <summary>Gets the Role ID that has Approved the Step last cleared by the document</summary>
        Int64 ApproveRole_ID { get; }
        /// <summary>Gets the Next Step ID that the Document requires to move up the Route.</summary>
        Int16 NextStep_ID { get; }
        /// <summary>Gets the Role that can push the document to the next step in the Route.</summary>
        Int64 NextRole_ID { get; }
        /// <summary>Sets the Approved Step that the document has cleared.</summary>
        /// <param name="ApprovedStep">The cleared step ID</param>
        void SetApproveStep_ID(Int16 ApprovedStep);
        /// <summary>Sets the Role that Approved the document.</summary>
        /// <param name="ApprovedRole">The Role ID</param>
        void SetApproveRole_ID(Int64 ApprovedRole);
        /// <summary>Sets the Next Step of the document.</summary>
        /// <param name="NextStep">The Next Step ID</param>
        void SetNextStep_ID(Int16 NextStep);
        /// <summary>Sets the Next Role for Approval.</summary>
        /// <param name="NextRole">The Role ID</param>
        void SetNextRole_ID(Int64 NextRole);
        /// <summary>Returns the Custom Validator Info</summary>
        WFValidatorInfo WFValidatorInfo { get; }
        /// <summary>Sets the Custom Validator Info</summary>
        /// <param name="WFVal">The Workflow Validator class info</param>
        void SetWFValidatorInfo(WFValidatorInfo WFVal);
        /// <summary>Sets the Field Access Table</summary>
        /// <param name="dtFieldAccess">The table that contains field access info</param>
        void SetFieldAccess(tblDocFieldAccess dtFieldAccess);
    }
}
